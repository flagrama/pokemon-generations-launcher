using System;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;

namespace PokeGen.Model
{
    static class GameNews
    {
        public struct NewsItem {
            public string NewsTextTitle;
            public string NewsTextDate;
            public string NewsTextLink;
            public string NewsPicLink;
            public BitmapImage NewsPicBitmap;
        };

        public static NewsItem LoadNews(int itemNumber) {
            var newsItem = new NewsItem();

            var htmlWeb = new HtmlWeb();
            var artNode = new HtmlDocument().DocumentNode;
            var artDateNode = new HtmlDocument().DocumentNode;
            var artNameNode = new HtmlDocument().DocumentNode;
            var link = new HtmlDocument().DocumentNode;
            var img = new HtmlDocument().DocumentNode;
            var src = "";
            var newsHtmlDocument = htmlWeb.Load("http://www.moddb.com/games/pokemon-generations/news");
            var imageHtmlDocument = htmlWeb.Load("http://www.moddb.com/games/pokemon-generations");
            var newDiv = imageHtmlDocument.DocumentNode.SelectSingleNode("//div[@class='mediapreview clear']");

            switch (itemNumber) {
                case 0:
                    artNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 1]");
                    artDateNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 2]");
                    link = newDiv.SelectSingleNode("(.//a[@href])[position() = 1]");
                    img = newDiv.SelectSingleNode("(.//img)[position() = 1]");
                    break;
                case 1:
                    artNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 2]");
                    artDateNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 3]");
                    link = newDiv.SelectSingleNode("(.//a[@href])[position() = 2]");
                    img = newDiv.SelectSingleNode("(.//img)[position() = 2]");
                    break;
                case 2:
                    artNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//h4)[position() = 3]");
                    artDateNode = newsHtmlDocument.DocumentNode.SelectSingleNode("(//span[@class='date'])[position() = 4]");
                    link = newDiv.SelectSingleNode("(.//a[@href])[position() = 3]");
                    img = newDiv.SelectSingleNode("(.//img)[position() = 3]");
                    break;
                default:
                    Log.WriteLog("Unable to load news items", Log.Type.Error);
                    break;

            }

            artNameNode = artNode.SelectSingleNode(".//a[@href]");
            src = img.Attributes["src"].Value;

            newsItem.NewsTextTitle = artNameNode.InnerHtml;
            newsItem.NewsTextLink = "http://www.moddb.com" + artNameNode.Attributes["href"].Value;
            newsItem.NewsTextDate = artDateNode.InnerHtml;
            newsItem.NewsPicLink = "http://www.moddb.com" + link.Attributes["href"].Value;
            newsItem.NewsPicBitmap = new BitmapImage(new Uri(src, UriKind.Absolute));


            return newsItem;
        }
    }
}
