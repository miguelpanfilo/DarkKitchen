using System.Text;

namespace ProductImporters.Tests;

[TestClass]
public class XmlProductImporterTest
{
    [TestMethod]
    public void Import_WhenXmlIsValid_ShouldReturnProducts()
    {
        var xml = """
                  <ArrayOfImportedProductDto>
                      <ImportedProductDto>
                          <Code>P001</Code>
                          <Name>Burger</Name>
                          <Description>Burger imported</Description>
                          <CommercialLine>Fast Food</CommercialLine>
                          <Category>Food</Category>
                          <Price>250</Price>
                          <PhotoPaths>
                              <string>C:\photos\burger.jpg</string>
                          </PhotoPaths>
                      </ImportedProductDto>
                  </ArrayOfImportedProductDto>
                  """;

        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(xml));

        var importer = new XmlProductImporter();

        var result = importer.Import(stream);

        result.Should().HaveCount(1);
        result[0].Code.Should().Be("P001");
        result[0].Name.Should().Be("Burger");
        result[0].Description.Should().Be("Burger imported");
        result[0].CommercialLine.Should().Be("Fast Food");
        result[0].Category.Should().Be("Food");
        result[0].Price.Should().Be(250);
        result[0].PhotoPaths.Should().ContainSingle()
            .Which.Should().Be(@"C:\photos\burger.jpg");
    }
}
