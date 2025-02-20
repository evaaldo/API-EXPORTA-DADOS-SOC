using API_INT_SOC_EXPORTA_DADOS.Models;
using System.Text;
using System.Xml.Linq;

namespace API_INT_SOC_EXPORTA_DADOS.Services
{
    public class SocService
    {
        private readonly ILogger<SocService> _logger;
        private readonly IConfiguration _configuration;

        public SocService(ILogger<SocService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string?> BuscarAsoPorPeriodo(AsoRequest parametros)
        {
            string soapXml = GerarSoapRequest(parametros);

            using (var client = new HttpClient())
            {
                var url = _configuration["Wsdl"] ?? throw new Exception("Wsdl não encontrado");

                var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");

                var response = await client.PostAsync(url, content);

                _logger.LogInformation("Status da requisição HTTP: " + (int)response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var xml = XDocument.Parse(responseString);
                    var erroTag = xml.Descendants().FirstOrDefault(x => x.Name.LocalName == "erro");

                    if (erroTag != null && erroTag.Value.Trim().ToLower() == "false")
                    {
                        var retornoTag = xml.Descendants().FirstOrDefault(x => x.Name.LocalName == "retorno");
                        var retornoXmlString = System.Web.HttpUtility.HtmlDecode(retornoTag.Value);
                        var retornoXml = XDocument.Parse(retornoXmlString);

                        var records = retornoXml.Descendants("record")
                            .Select(record => new
                            {
                                CPF = record.Element("CPFFUNCIONARIO")?.Value,
                                Nome = record.Element("NOMEFUNCIONARIO")?.Value,
                                TPASO = record.Element("TPASO")?.Value,
                                CODPARECERASO = record.Element("CODPARECERASO")?.Value,
                                DataASO = record.Element("DTASO")?.Value,
                                DATAFICHA = record.Element("DATAFICHA")?.Value,
                                CRM = record.Element("CRM")?.Value,
                                UF = record.Element("UF")?.Value
                            }).ToList().Where(x => x.CPF == parametros.cpf).Where(x => x.TPASO == "5" || x.TPASO == "2").Where(x => x.CODPARECERASO == "1");

                        if (!records.Any())
                        {
                            return "Não existe nenhum exame demissional ou periódico com OK para o período e CPF informados";
                        }

                        return System.Text.Json.JsonSerializer.Serialize(records);
                    }
                    else
                    {
                        var messageErroTag = xml.Descendants().FirstOrDefault(x => x.Name.LocalName == "mensagemErro");
                        _logger.LogError("Houve um erro na requisição com o SOC. Erro: " + messageErroTag?.Value.Trim());
                        return messageErroTag?.Value.Trim();
                    }
                }
                else
                {
                    throw new Exception("Erro ao realizar requisição SOAP.");
                }
            };
        }

        private static string GerarSoapRequest(AsoRequest parametros)
        {
            return $@"
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ser=""http://services.soc.age.com/"">
                <soapenv:Header/>
                <soapenv:Body>
                    <ser:exportaDadosWs>
                        <arg0>
                            <arquivo></arquivo>
                            <campoLivre1></campoLivre1>
                            <campoLivre2></campoLivre2>
                            <campoLivre3></campoLivre3>
                            <campoLivre4></campoLivre4>
                            <campoLivre5></campoLivre5>
                            <erro></erro>
                            <mensagemErro></mensagemErro>
                            <parametros>{System.Text.Json.JsonSerializer.Serialize(parametros)}</parametros>
                            <retorno></retorno>
                            <tipoArquivoRetorno>xml</tipoArquivoRetorno>
                        </arg0>
                    </ser:exportaDadosWs>
                </soapenv:Body>
            </soapenv:Envelope>";
        }
    }
}
