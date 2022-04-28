using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Samples.Console.Tests
{
    public class TextJsonCustomTest : ITest
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        public class ServerInfo
        {
            /// <summary>
            /// 服务器名称
            /// </summary>
            [JsonPropertyName("server_name")]
            public string ServerName { get; set; }

            /// <summary>
            /// 服务器ID
            /// </summary>
            [JsonPropertyName("server_id")]
            public int ServerID { get; set; }

            /// <summary>
            /// start_time
            /// </summary>
            [JsonConverter(typeof(CustomDateTimeConverter))]
            [JsonPropertyName("start_time")]
            public DateTimeOffset StartTime { get; set; }
        }

        public class CustomDateTimeConverter : JsonConverter<DateTimeOffset>
        {
            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var content = reader.GetString();

                if (long.TryParse(content, out var result))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(result);
                }
                else
                {
                    return new DateTimeOffset();
                }
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToUnixTimeSeconds().ToString());
            }
        }

        public void DoTest()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var serverInfo = new ServerInfo()
            {
                ServerName = "服务器名称",
                ServerID = 1,
                StartTime = DateTime.Now
            };

            var content = JsonSerializer.Serialize(serverInfo, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            System.Console.WriteLine(content);

            var contentValue = JsonSerializer.Deserialize<ServerInfo>(content);
        }
    }
}
