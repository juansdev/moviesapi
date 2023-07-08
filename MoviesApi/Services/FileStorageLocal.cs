namespace MoviesApi.Services;

public class FileStorageLocal : IFileStorage
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileStorageLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
    {
        var fileName = $"{Guid.NewGuid()}{extension}";
        var folder = Path.Combine(_env.WebRootPath, container);

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, fileName);
        await File.WriteAllBytesAsync(path, content);

        var actualUrl =
            $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        var urlForDb = Path.Combine(actualUrl, container, fileName).Replace("\\", "/");
        return urlForDb;
    }

    public async Task<string> EditFile(byte[] content, string extension, string container, string path,
        string contentType)
    {
        await DeleteFile(path, container);
        return await SaveFile(content, extension, container, contentType);
    }

    public Task DeleteFile(string path, string container)
    {
        if (path != null)
        {
            var fileName = Path.GetFileName(path);
            var fileDirectory = Path.Combine(_env.WebRootPath, container, fileName);
            if (File.Exists(fileDirectory)) File.Delete(fileDirectory);
        }

        return Task.FromResult(0);
    }
}