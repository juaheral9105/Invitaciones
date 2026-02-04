using OfficeOpenXml;
using InvitacionesAPI.Models;

namespace InvitacionesAPI.Services
{
    public class ExcelParserService
    {
        public ExcelParserService()
        {
            // EPPlus requires a license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<List<GuestListItem>> ParseExcelFile(Stream fileStream, Guid invitationId)
        {
            var guestList = new List<GuestListItem>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // First sheet
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                // Expected columns:
                // A: Nombre
                // B: Celular (Phone)
                // C: Número de invitados
                // D: Nombre de invitados
                // E: Evento tiene hospedaje
                // F: Hospedaje permitido

                // Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    var name = worksheet.Cells[row, 1].Text;
                    var phone = worksheet.Cells[row, 2].Text;

                    // Skip empty rows
                    if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(phone))
                        continue;

                    var guestNamesText = worksheet.Cells[row, 4].Text?.Trim();

                    var guest = new GuestListItem
                    {
                        InvitationId = invitationId,
                        Name = name,
                        Phone = phone,
                        NumberOfGuests = ParseInt(worksheet.Cells[row, 3].Text),
                        GuestNames = string.IsNullOrWhiteSpace(guestNamesText) ? null : guestNamesText,
                        EventHasAccommodation = ParseBoolean(worksheet.Cells[row, 5].Text),
                        AccommodationAllowed = ParseBoolean(worksheet.Cells[row, 6].Text),
                        CreatedAt = DateTime.UtcNow
                    };

                    guestList.Add(guest);
                }
            }

            return guestList;
        }

        private int ParseInt(string value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        private bool ParseBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim().ToLower();

            // Accept variations: "si", "yes", "true", "1", "sí"
            return value == "si" || value == "sí" || value == "yes" ||
                   value == "true" || value == "1";
        }
    }
}
