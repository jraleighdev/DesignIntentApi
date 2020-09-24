using Domain.Interfaces;
using Domain.Models.CouldStorage;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CloudFileService : ICloudFilesService
    {
        private readonly DataContext _context;

        public CloudFileService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddFile(CloudFile file)
        {
            _context.CloudFiles.Add(file);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<CloudFile> FindFile(string name)
        {
            var file = await _context.CloudFiles.FirstOrDefaultAsync(x => x.FileName.ToUpper().Contains(name));

            return file;
        }

        public async Task<CloudFile> GetFile(Guid id)
        {
            return await _context.CloudFiles.Include(x => x.Bill).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<CloudFile>> GetFiles(string find = "")
        {
            if (string.IsNullOrEmpty(find))
            {
                return await _context.CloudFiles.Include(x => x.Bill).ToListAsync();
            }

            return await _context.CloudFiles.Include(x => x.Bill).Where(x => x.FileName.Contains(find)).ToListAsync();
        }

        public async Task<CloudFile> UpdateFile(CloudFile file)
        {
            var fileInDb = _context.CloudFiles.Include(x => x.Bill).FirstOrDefault(x => x.Id == file.Id);

            if (fileInDb == null)
            {
                return null;
            }

            _context.Entry(fileInDb).CurrentValues.SetValues(file);

            // handle delete
            foreach (var billInDb in fileInDb.Bill.ToList())
            {
                if (!file.Bill.Any(x => x.Id == billInDb.Id))
                {
                    _context.Bills.Remove(billInDb);
                }
            }

            foreach (var bill in file.Bill)
            {
                var billInDb = _context.Bills.FirstOrDefault(x => x.Id == bill.Id);

                if (billInDb != null)
                {
                    _context.Entry(billInDb).CurrentValues.SetValues(bill);
                }
                else
                {
                    var newBill = new BillItem
                    {
                        // Id = Guid.NewGuid(),
                        Description = bill.Description,
                        Length = bill.Length,
                        Name = bill.Name,
                        PartNumber = bill.PartNumber,
                        Qty = bill.Qty,
                        StockNumber = bill.StockNumber
                    };

                    fileInDb.Bill.Add(newBill);
                }
            }


            await _context.SaveChangesAsync();

            return file;
        }

    }
}
