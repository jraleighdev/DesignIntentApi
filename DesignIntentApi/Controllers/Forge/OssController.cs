﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesignIntentApi.Controllers.Forge
{
    [Route("api/[controller]")]
    [ApiController]
    public class OssController : ControllerBase
    {
        private IWebHostEnvironment _env;
        public OssController(IWebHostEnvironment env) { _env = env; }
        public string ClientId { get { return ForgeAuthController.GetAppSetting("FORGE_CLIENT_ID").ToLower(); } }

        /// <summary>
        /// Return list of buckets (id=#) or list of objects (id=bucketKey)
        /// </summary>
        [Authorize]
        [HttpGet("buckets")]
        public async Task<IList<TreeNode>> GetOSSAsync()
        {
            IList<TreeNode> nodes = new List<TreeNode>();
            dynamic oauth = await ForgeAuthController.GetInternalAsync();

            // in this case, let's return all buckets
            BucketsApi appBckets = new BucketsApi();
            appBckets.Configuration.AccessToken = oauth.access_token;

            // to simplify, let's return only the first 100 buckets
            dynamic buckets = await appBckets.GetBucketsAsync("US", 100);
            foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
            {
                nodes.Add(new TreeNode(bucket.Value.bucketKey, bucket.Value.bucketKey.Replace(ClientId + "-", string.Empty), "bucket", true));
            }

            return nodes;
        }

        [Authorize]
        [HttpGet("getobjects/{id}")]
        public async Task<IList<TreeNode>> GetListOfBuckets(string id)
        {
            IList<TreeNode> nodes = new List<TreeNode>();
            dynamic oauth = await ForgeAuthController.GetInternalAsync();

            // as we have the id (bucketKey), let's return all 
            ObjectsApi objects = new ObjectsApi();
            objects.Configuration.AccessToken = oauth.access_token;
            var objectsList = objects.GetObjects(id);
            foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
            {
                nodes.Add(new TreeNode(Base64Encode((string)objInfo.Value.objectId),
                  objInfo.Value.objectKey, "object", false));
            }

            return nodes;
        }

        /// <summary>
        /// Model data for jsTree used on GetOSSAsync
        /// </summary>
        public class TreeNode
        {
            public TreeNode(string id, string text, string type, bool children)
            {
                this.id = id;
                this.text = text;
                this.type = type;
                this.children = children;
            }

            public string id { get; set; }
            public string text { get; set; }
            public string type { get; set; }
            public bool children { get; set; }
        }

        /// <summary>
        /// Create a new bucket 
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("forge/oss/buckets")]
        public async Task<dynamic> CreateBucket([FromBody] CreateBucketModel bucket)
        {
            BucketsApi buckets = new BucketsApi();
            dynamic token = await ForgeAuthController.GetInternalAsync();
            buckets.Configuration.AccessToken = token.access_token;
            PostBucketsPayload bucketPayload = new PostBucketsPayload(string.Format("{0}-{1}", ClientId, bucket.bucketKey.ToLower()), null,
              PostBucketsPayload.PolicyKeyEnum.Transient);
            return await buckets.CreateBucketAsync(bucketPayload, "US");
        }

        /// <summary>
        /// Input model for CreateBucket method
        /// </summary>
        public class CreateBucketModel
        {
            public string bucketKey { get; set; }
        }

        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("forge/oss/objects")]
        public async Task<dynamic> UploadObject([FromForm] UploadFile input)
        {
            // save the file on the server
            var fileSavePath = Path.Combine(_env.WebRootPath, Path.GetFileName(input.fileToUpload.FileName));
            using (var stream = new FileStream(fileSavePath, FileMode.Create))
                await input.fileToUpload.CopyToAsync(stream);


            // get the bucket...
            dynamic oauth = await ForgeAuthController.GetInternalAsync();
            ObjectsApi objects = new ObjectsApi();
            objects.Configuration.AccessToken = oauth.access_token;

            // upload the file/object, which will create a new object
            dynamic uploadedObj;
            using (StreamReader streamReader = new StreamReader(fileSavePath))
            {
                uploadedObj = await objects.UploadObjectAsync(input.bucketKey,
                       Path.GetFileName(input.fileToUpload.FileName), (int)streamReader.BaseStream.Length, streamReader.BaseStream,
                       "application/octet-stream");
            }

            // cleanup
            System.IO.File.Delete(fileSavePath);

            return uploadedObj;
        }

        public class UploadFile
        {
            public string bucketKey { get; set; }
            public IFormFile fileToUpload { get; set; }
        }

        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}