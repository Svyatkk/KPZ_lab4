using System;

namespace Lab4.Task4_Strategy
{
    public interface IImageLoadStrategy
    {
        string Load(string href);
    }

    public class FileSystemLoadStrategy : IImageLoadStrategy
    {
        public string Load(string href)
        {
            Console.WriteLine($"[Strategy: FileSystem] Завантажую зображення з диску: '{href}'");
            return $"[FILE_DATA from '{href}']";
        }
    }

    public class NetworkLoadStrategy : IImageLoadStrategy
    {
        public string Load(string href)
        {
            Console.WriteLine($"[Strategy: Network] Завантажую зображення з мережі: '{href}'");
            return $"[NETWORK_DATA from '{href}']";
        }
    }

    public static class ImageLoadStrategyFactory
    {
        public static IImageLoadStrategy Resolve(string href)
        {
            if (href.StartsWith("http://") || href.StartsWith("https://"))
                return new NetworkLoadStrategy();

            return new FileSystemLoadStrategy();
        }
    }

    public class LightImage
    {
        public string Href { get; }
        public string Alt { get; }
        private readonly IImageLoadStrategy _strategy;
        private string? _cachedData;

        public LightImage(string href, string alt = "")
        {
            Href = href;
            Alt = alt;
            _strategy = ImageLoadStrategyFactory.Resolve(href);

            Console.WriteLine($"[LightImage] Обрано стратегію: {_strategy.GetType().Name} для '{href}'");
        }

        public string LoadImage()
        {
            _cachedData ??= _strategy.Load(Href);
            return _cachedData;
        }

        public string OuterHtml()
        {
            string data = LoadImage();
            return $"<img src=\"{Href}\" alt=\"{Alt}\" data=\"{data}\" />";
        }
    }
}