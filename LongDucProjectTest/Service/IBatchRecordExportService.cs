using System;

namespace LongDucProjectTest.Service
{
    public interface IBatchRecordExportService
    {
        /// <summary>
        /// Exports the Batch Production Record to a byte array based on a template Excel file.
        /// </summary>
        /// <param name="batchId">The ID of the batch to export.</param>
        /// <param name="fileName">Outputs the recommended file name.</param>
        /// <returns>Excel file byte array.</returns>
        byte[] ExportBatchRecord(int batchId, out string fileName);
    }
}
