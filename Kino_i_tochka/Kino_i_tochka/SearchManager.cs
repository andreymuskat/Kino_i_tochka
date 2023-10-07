using HtmlAgilityPack;

class SearchManager
{
    public async Task<string> DownloadHttpKinopoiskFilm(string nameFilm)
    {
        string Url = "https://www.kinopoisk.ru/index.php?kp_query=" + nameFilm.Replace(" ", "+"); ;
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(Url);

            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();

                return htmlContent;
            }

            return "Ничего не найдено";
        }
    }



    public async Task<string> SearchNameFilm(string inputNameFilm)
    {
        string htmlContent = await DownloadHttpKinopoiskFilm(inputNameFilm);

        // Создайте объект HtmlDocument и загрузите в него HTML-контент
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        // Найдите элемент <p> с классом "name"
        HtmlNode pElement = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='name']");

        if (pElement != null)
        {
            // Найдите внутри элемента <p> элемент <a> с классом "js-serp-metrika"
            HtmlNode aElement = pElement.SelectSingleNode(".//a[@class='js-serp-metrika']");
            HtmlNode spanElement = pElement.SelectSingleNode(".//span[@class='year']");
            if (aElement != null)
            {
                // Получите значение атрибута "href" элемента <a>
                string outputNameFilm = aElement.InnerText;
                string year = spanElement.InnerText;
                string result;
                if (outputNameFilm != inputNameFilm)
                {
                    result = "Возможно вы имели ввиду: " + outputNameFilm + " " + year;
                }
                else
                    result = outputNameFilm + " " + year;

                return result;
            }
        }

        return "Ничего не найдено";
    }

    public async Task<string> SearchUrlFilm(string nameFilm)
    {
        string htmlContent = await DownloadHttpKinopoiskFilm(nameFilm);

        HtmlNode aElement = await SearchTheATagWithTheJsSerpMetrikaClass(htmlContent);
        if (aElement != null)
        {
            // Получите значение атрибута "href" элемента <a>
            string href = aElement.GetAttributeValue("href", "");
            string result;

            result = "https://1ww.frkp.live/" + href.Replace("/sr/1/", "");

            return result;
        }

        return "Ничего не найдено";
    }

    public async Task<string> SearchIdFilm(string htmlContent)
    {

        HtmlNode aElement = await SearchTheATagWithTheJsSerpMetrikaClass(htmlContent);
        if (aElement != null)
        {
            // Получите значение атрибута "href" элемента <a>
            string href = aElement.GetAttributeValue("href", "");
            href = href.Replace("/film/", "");

            string result = href.Replace("/sr/1/", "");

            return result;
        }

        return null;
    }

    public async Task<string> DownloadUrlImgFilm(string nameFilm)
    {
        string htmlContent = await DownloadHttpKinopoiskFilm(nameFilm);

        string id = await SearchIdFilm(htmlContent);

        if (id != null)
        {
            string result = "https://st.kp.yandex.net/images/film_iphone/iphone360_" + id + ".jpg";

            return result;
        }

        return null;
    }

    public async Task<HtmlNode> SearchTheATagWithTheJsSerpMetrikaClass(string htmlContent)
    {
        // Создайте объект HtmlDocument и загрузите в него HTML-контент
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        // Найдите элемент <p> с классом "name"
        HtmlNode pElement = htmlDocument.DocumentNode.SelectSingleNode("//p[@class='name']");

        if (pElement != null)
        {
            // Найдите внутри элемента <p> элемент <a> с классом "js-serp-metrika"
            HtmlNode aElement = pElement.SelectSingleNode(".//a[@class='js-serp-metrika']");

            return aElement;

        }

        return null;
    }

}



