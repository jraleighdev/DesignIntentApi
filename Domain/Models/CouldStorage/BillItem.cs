using System;

namespace Domain.Models.CouldStorage
{
    public class BillItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PartNumber { get; set; }

        public string Description { get; set; }

        public string StockNumber { get; set; }

        public int Qty { get; set; }

        public double Length { get; set; }
    }
}
