using Amazon.S3;
using Amazon.S3.Model;
using Garagem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Garagem.Infrastructure.Storage;

/// <summary>
/// Serviço de armazenamento que implementa a interface IStorageService
/// Usa AWS S3 SDK que é compatível com MinIO
/// </summary>
public class MinioStorageService : IStorageService
{
	private readonly IAmazonS3 _s3Client;
	private readonly string _bucketName;
	private readonly string _endpoint;

	public MinioStorageService(
		IAmazonS3 s3Client,
		IConfiguration configuration)
	{
		_s3Client = s3Client;
		_bucketName = configuration["Minio:BucketName"]
			?? throw new Exception("Minio:BucketName não configurado");
		_endpoint = configuration["Minio:Endpoint"]
			?? throw new Exception("Minio:Endpoint não configurado");
	}

	public async Task<string> UploadAsync(
		string fileName,
		Stream fileStream,
		string contentType,
		CancellationToken cancellationToken = default)
	{
		try
		{
			// Gera um nome único para evitar conflitos
			var uniqueFileName = GenerateUniqueFileName(fileName);

			// Cria request de upload
			var putRequest = new PutObjectRequest
			{
				BucketName = _bucketName,
				Key = uniqueFileName,
				InputStream = fileStream,
				ContentType = contentType
			};

			await _s3Client.PutObjectAsync(putRequest, cancellationToken);

			// Retorna a URL do arquivo no MinIO
			return GeneratePublicUrl(uniqueFileName);
		}
		catch (AmazonS3Exception ex)
		{
			throw new Exception($"Erro ao fazer upload no MinIO: {ex.Message}", ex);
		}
		catch (Exception ex)
		{
			throw new Exception($"Erro inesperado ao fazer upload: {ex.Message}", ex);
		}
	}

	public async Task DeleteAsync(
		string fileUrl,
		CancellationToken cancellationToken = default)
	{
		try
		{
			// Extrai o nome do arquivo da URL
			var fileName = ExtractFileNameFromUrl(fileUrl);

			var deleteRequest = new DeleteObjectRequest
			{
				BucketName = _bucketName,
				Key = fileName
			};

			await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
		}
		catch (AmazonS3Exception ex)
		{
			throw new Exception($"Erro ao deletar arquivo do MinIO: {ex.Message}", ex);
		}
		catch (Exception ex)
		{
			throw new Exception($"Erro inesperado ao deletar arquivo: {ex.Message}", ex);
		}
	}

	public string GeneratePresignedUrl(
		string fileName,
		int expirationTime = 1440)
	{
		try
		{
			var request = new GetPreSignedUrlRequest
			{
				BucketName = _bucketName,
				Key = fileName,
				Expires = DateTime.UtcNow.AddMinutes(expirationTime),
				Verb = HttpVerb.GET
			};

			return _s3Client.GetPreSignedURL(request);
		}
		catch (Exception ex)
		{
			throw new Exception($"Erro ao gerar URL assinada: {ex.Message}", ex);
		}
	}

	/// <summary>
	/// Gera um nome único para o arquivo usando timestamp e GUID
	/// </summary>
	private static string GenerateUniqueFileName(string originalFileName)
	{
		var timestamp = DateTime.UtcNow.Ticks;
		var extension = Path.GetExtension(originalFileName);
		var guid = Guid.NewGuid().ToString().Substring(0, 8);

		return $"profile-images/{guid}-{timestamp}{extension}";
	}

	/// <summary>
	/// Extrai o nome do arquivo da URL do MinIO
	/// </summary>
	private string ExtractFileNameFromUrl(string fileUrl)
	{
		if (fileUrl.StartsWith(_bucketName))
			return fileUrl.Substring(_bucketName.Length).TrimStart('/');

		// Se a URL contém o endpoint, extrai a parte após o bucket
		if (fileUrl.Contains(_bucketName))
		{
			var parts = fileUrl.Split(new[] { _bucketName }, StringSplitOptions.None);
			return parts.Length > 1 ? parts[1].TrimStart('/') : fileUrl;
		}

		return fileUrl;
	}

	/// <summary>
	/// Gera a URL pública do arquivo no MinIO
	/// </summary>
	private string GeneratePublicUrl(string fileName)
	{
		var baseUrl = _endpoint.TrimEnd('/');
		return $"{baseUrl}/{_bucketName}/{fileName}";
	}
}
