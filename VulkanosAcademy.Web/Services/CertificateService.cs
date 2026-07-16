using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface ICertificateService
{
    Task<CertificateResponseDto?> GenerateCertificateAsync(Guid enrollmentId);
    Task<CertificateDto?> GetCertificateAsync(Guid certificateId);
    Task<IEnumerable<CertificateDto>?> GetUserCertificatesAsync(Guid userId);
    Task<bool> CertificateExistsAsync(Guid enrollmentId);
    Task<bool> DownloadCertificateAsync(Guid certificateId, string fileName);
}

public class CertificateService : ICertificateService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CertificateService> _logger;

    public CertificateService(HttpClient httpClient, ILogger<CertificateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CertificateResponseDto?> GenerateCertificateAsync(Guid enrollmentId)
    {
        try
        {
            var generateDto = new GenerateCertificateDto { EnrollmentId = enrollmentId };
            var response = await _httpClient.PostAsJsonAsync("api/certificates/generate", generateDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<CertificateResponseDto>();
            }

            _logger.LogWarning($"Erro ao gerar certificado: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao gerar certificado: {ex.Message}");
            return null;
        }
    }

    public async Task<CertificateDto?> GetCertificateAsync(Guid certificateId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CertificateDto>($"api/certificates/{certificateId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificado: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<CertificateDto>?> GetUserCertificatesAsync(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CertificateDto>>($"api/certificates/user/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificados do usuário: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CertificateExistsAsync(Guid enrollmentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/certificates/enrollment/{enrollmentId}/exists");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<bool>();
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar existência do certificado: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DownloadCertificateAsync(Guid certificateId, string fileName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/certificates/{certificateId}/download");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                
                // Simular download (em um navegador real, isso seria tratado diferentemente)
                _logger.LogInformation($"Certificado {certificateId} pronto para download");
                return true;
            }

            _logger.LogWarning($"Erro ao fazer download do certificado: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao fazer download do certificado: {ex.Message}");
            return false;
        }
    }
}

public class GenerateCertificateDto
{
    public Guid EnrollmentId { get; set; }
}
