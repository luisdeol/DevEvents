using DevEvents.API.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DevEvents.API.Infrastructure.Reports
{
    public class ConferenceDetailsReport : IDocument
    {
        public Conference Model { get; set; }
        public ConferenceDetailsReport(Conference conference)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Model = conference;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .AlignCenter()
                    .Text($"Conference Report: {Model.Title}")
                    .FontSize(24)
                    .SemiBold();

                page.Content().Column(col =>
                {
                    // Conference Details
                    col.Item().Text("Title").FontSize(14).Bold();
                    col.Item().PaddingBottom(10).Text(Model.Title);

                    col.Item().Text("Description").FontSize(14).Bold();
                    col.Item().PaddingBottom(10).Text(Model.Description);

                    col.Item().Text("Start Date").FontSize(14).Bold();
                    col.Item().PaddingBottom(10).Text(Model.StartDate.ToString("g"));

                    col.Item().Text("End Date").FontSize(14).Bold();
                    col.Item().PaddingBottom(10).Text(Model.EndDate.ToString("g"));

                    // Speakers
                    col.Item().Text("Speakers").FontSize(16).Bold();

                    if (Model.Speakers.Any())
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                                columns.ConstantColumn(150);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeadersCellStyle).Text("Name").Bold();
                                header.Cell().Element(HeadersCellStyle).Text("Bio").Bold();
                                header.Cell().Element(HeadersCellStyle).Text("Website").Bold();
                            });

                            foreach (var speaker in Model.Speakers)
                            {
                                table.Cell().Text(speaker.Name);
                                table.Cell().Text(speaker.Bio);
                                table.Cell().Text(speaker.Website);
                            }
                        });
                    } else
                    {
                        col.Item().Text("No speakers registered.").Italic();
                    }

                    col.Item().PaddingBottom(10);

                    // Registations
                    col.Item().Text("Registrations").FontSize(16).Bold();

                    if (Model.Registrations.Any())
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeadersCellStyle).Text("Name").Bold();
                                header.Cell().Element(HeadersCellStyle).Text("Email").Bold();
                            });

                            foreach (var registration in Model.Registrations)
                            {
                                table.Cell().Text(registration.Attendee.Name);
                                table.Cell().Text(registration.Attendee.Email);
                            }
                        });
                    }
                    else
                    {
                        col.Item().Text("No registrations.").Italic();
                    }

                    col.Item().PaddingBottom(10);

                    // Summary
                    col.Item().Text("Summary").FontSize(16).Bold();
                    col.Item().Text($"Total speakers: {Model.Speakers.Count}");
                    col.Item().Text($"Total Registrations: {Model.Registrations.Count}");
                });

                page.Footer()
                    .AlignCenter()
                    .Text(t =>
                    {
                        t.CurrentPageNumber();
                        t.Span("/");
                        t.TotalPages();
                        t.EmptyLine();
                        t.Line($"Date: {DateTime.UtcNow:MM-dd-yyyy HH:mm}");
                    });
            });

        }

        static IContainer HeadersCellStyle(IContainer container)
        {
            return container.DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(5)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
        }
    }
}
