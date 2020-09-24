using Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.CouldStorage
{
    public class CloudFile
    {
        public Guid Id { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public bool ThreeDfile { get; set; }

        public bool DrawingFile { get; set; }

        public string ForgeUrn { get; set; }

        public string ForgeBucket { get; set; }

        public ICollection<BillItem> Bill { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }
    }
}
