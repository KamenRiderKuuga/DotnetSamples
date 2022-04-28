using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Samples.Console.Tests
{
    public class HTMLReaderTest : ITest
    {
        public void DoTest()
        {
            var result = string.Empty;
            var spanStack = new Stack<(bool, bool)>();
            var content = @"<p><span style=""font-size: 18px;""><strong>亲爱的各位玩家：</strong></span></p>
<p><span style=""font-size: 18px;"">&nbsp; &nbsp; 我们将于<em><strong>北京时间2021年8月4日</strong> </em>对逍遥情缘首次开启删档公测，希望大家踊跃参与。我们准备了丰富的公测庆典活动，特殊配件、永久棋盘、S级棋手等你来拿。</span></p>
<p><span style=""font-size: 18px;"">&nbsp; &nbsp; 更多版本爆料,请留意<span style=""color: #ff0000;"">社区</span>和<span style=""color: #ff0000;"">后续公告</span>.</span></p>
<p><span style=""font-size: 18px;"">&nbsp; &nbsp; 游戏品质的改进离不开各位的协助，如果各位在游戏中有任何疑问，可以使用游戏中的&ldquo;<strong>联系客服</strong>&rdquo;功能进行反馈，或者可以在玩家社群中与更多玩家共同讨论哦！</span></p>
<p><span style=""font-size: 18px;"">&nbsp; &nbsp; 感谢您的支持，祝您游戏愉快！</span></p>
<p>&nbsp;</p>
<p><span style=""font-size: 18px;"">&nbsp; &nbsp;《逍遥情缘》运营团队</span></p>";

            content = @$"<!DOCTYPE documentElement[
                      <!ENTITY Alpha ""&#913;"">
                      <!ENTITY ndash ""&#8211;"">
                      <!ENTITY mdash ""&#8212;"">
                      <!ENTITY nbsp ""\u00A0\u00A0"">
                      <!ENTITY ldquo ""&#8220;"">
                      <!ENTITY rdquo ""&#8221;"">
                      ]><root>{content}</root>";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using (XmlReader reader = XmlReader.Create(new StringReader(content), settings))
            {
                while (reader.Read())
                {
                    var tag = reader.Name;
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            System.Diagnostics.Debug.WriteLine("Start Element {0}", reader.Name);

                            if (tag == "strong")
                            {
                                result += "<b>";
                            }
                            else if (tag == "em")
                            {
                                result += "<i>";
                            }
                            else if (tag == "span")
                            {
                                var style = reader.GetAttribute("style");
                                var hasColor = false;
                                var hasSize = false;
                                if (!string.IsNullOrEmpty(style))
                                {
                                    if (style.Contains("color:"))
                                    {
                                        var matchResult = Regex.Match(style, "color: (.*?);");
                                        if (matchResult.Success && matchResult.Groups.Count > 1)
                                        {
                                            result += $"<color={matchResult.Groups[1].Value}>";
                                            hasColor = true;
                                        }
                                    }

                                    if (style.Contains("font-size:"))
                                    {
                                        var matchResult = Regex.Match(style, "font-size: (.*?);");
                                        if (matchResult.Success && matchResult.Groups.Count > 1)
                                        {
                                            result += $"<size={matchResult.Groups[1].Value}>";
                                            hasSize = true;
                                        }
                                    }

                                    spanStack.Push((hasColor, hasSize));
                                }
                            }

                            break;
                        case XmlNodeType.Text:
                            System.Diagnostics.Debug.WriteLine("Text Node: {0}", reader.Value);
                            result += reader.Value;

                            break;
                        case XmlNodeType.EndElement:
                            System.Diagnostics.Debug.WriteLine("End Element {0}", reader.Name);

                            if (tag == "p")
                            {
                                result += "\\n";
                            }
                            else if (tag == "span")
                            {
                                (var hasColor, var hasSize) = spanStack.Pop();

                                if (hasSize)
                                {
                                    result += "</size>";
                                }

                                if (hasColor)
                                {
                                    result += "</color>";
                                }
                            }
                            else if (tag == "strong")
                            {
                                result += "</b>";
                            }
                            else if (tag == "em")
                            {
                                result += "</i>";
                            }

                            break;

                        default:
                            // do nothing
                            break;
                    }
                }

                System.Diagnostics.Debug.WriteLine(result);
            }
        }
    }
}
