namespace Garagem.Domain.Interfaces;

/// <summary>
/// Interface para abstrair o serviço de armazenamento (S3/MinIO)
/// </summary>
public interface IStorageService
{
	/// <summary>
	/// Faz upload de um arquivo e retorna a URL pública
	/// </summary>
	/// <param name="fileName">Nome do arquivo</param>
	/// <param name="fileStream">Stream do arquivo</param>
	/// <param name="contentType">Tipo MIME do arquivo</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	/// <returns>URL pública do arquivo</returns>
	Task<string> UploadAsync(
		string fileName,
		Stream fileStream,
		string contentType,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Remove um arquivo do armazenamento
	/// </summary>
	/// <param name="fileUrl">URL do arquivo ou nome do arquivo</param>
	/// <param name="cancellationToken">Token de cancelamento</param>
	Task DeleteAsync(
		string fileUrl,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Gera uma URL assinada/segura para o arquivo
	/// </summary>
	/// <param name="fileName">Nome do arquivo</param>
	/// <param name="expirationTime">Tempo de expiração em minutos</param>
	/// <returns>URL assinada</returns>
	string GeneratePresignedUrl(
		string fileName,
		int expirationTime = 1440);
}
