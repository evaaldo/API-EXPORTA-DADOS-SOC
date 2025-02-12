using API_INT_SOC_EXPORTA_DADOS.Models;
using System.Text;
using System.Xml.Linq;

namespace API_INT_SOC_EXPORTA_DADOS.Services
{
    public class SocService
    {
        private readonly ILogger<SocService> _logger;

        public SocService(ILogger<SocService> logger)
        {
            _logger = logger;
        }

        public async Task<string> BuscarAsoPorPeriodo(AsoPeriodo parametros)
        {
            string soapXml = $@"
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

            using (var client = new HttpClient())
            {
                var url = "https://www.p-soc.com.br/WSSoc/services/ExportaDadosWs";

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
                        _logger.LogInformation("Status da requisição para o SOC: 200");
                        return responseString;
                    }
                    else
                    {
                        _logger.LogError("Houve um erro na requisição com o SOC. Verifique os parâmetros passados.");
                        return "erro";
                    }
                }
                else
                {
                    throw new Exception("Erro ao realizar requisição SOAP.");
                }
            };
        }
    }
}
