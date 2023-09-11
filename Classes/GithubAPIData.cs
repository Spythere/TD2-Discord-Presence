using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TD2_Presence.Classes
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse)
    public record Asset(
      [property: JsonPropertyName("url")] string Url,
      [property: JsonPropertyName("id")] int Id,
      [property: JsonPropertyName("node_id")] string NodeId,
      [property: JsonPropertyName("name")] string Name,
      [property: JsonPropertyName("label")] object Label,
      [property: JsonPropertyName("content_type")] string ContentType,
      [property: JsonPropertyName("state")] string State,
      [property: JsonPropertyName("size")] int Size,
      [property: JsonPropertyName("download_count")] int DownloadCount,
      [property: JsonPropertyName("created_at")] DateTime CreatedAt,
      [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
      [property: JsonPropertyName("browser_download_url")] string BrowserDownloadUrl
  );

    public record GithubAPIReleaseData(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("assets_url")] string AssetsUrl,
        [property: JsonPropertyName("upload_url")] string UploadUrl,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("node_id")] string NodeId,
        [property: JsonPropertyName("tag_name")] string TagName,
        [property: JsonPropertyName("target_commitish")] string TargetCommitish,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("draft")] bool Draft,
        [property: JsonPropertyName("prerelease")] bool Prerelease,
        [property: JsonPropertyName("assets")] IReadOnlyList<Asset> Assets,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("published_at")] DateTime PublishedAt,
        [property: JsonPropertyName("tarball_url")] string TarballUrl,
        [property: JsonPropertyName("zipball_url")] string ZipballUrl,
        [property: JsonPropertyName("body")] string Body
    );
}
