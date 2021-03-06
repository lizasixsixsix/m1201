# 12 HTTP fundamentals

## задание

Необходимо реализовать библиотеку и использующую её консольную программу
для создания локальной копии сайта
(«аналог» программы [wget](https://ru.wikipedia.org/wiki/Wget)).

Работа с программой выглядит так:
пользователь указывает стартовую точку (URL) и папку куда надо сохранять,
а программа проходит по всем доступным ссылкам и рекурсивно выкачивает сайт(ы).

Опции программы/библиотеки:

*   ограничение на глубину анализа ссылок
    (т.е. если вы скачали страницу, которую указал пользователь, &mdash;
    это уровень 0; все страницы, на которые введут ссылки с неё, &mdash;
    это уровень 1, и т.д.)

*   ограничение на переход на другие домены
    (без ограничений/только внутри текущего домена/не выше пути в исходном URL)

*   ограничение на расширение скачиваемых ресурсов
    (можно задавать списком, например так: `gif,jpeg,jpg,pdf`)

*   трассировка (_verbose_ режим):
    показ на экране текущей обрабатываемой страницы/документа

## рекомендации по реализации

В качестве основы можно взять следующие библиотеки:

*   Работа с HTTP:

    *   System.Net.Http.HttpClient &mdash; рекомендуемый вариант:

        *   Если вы работаете с _.NET_ 4.5+, он включен в сам фреймворк.
            В более ранних версиях и для прочих платформ получите через
            [_NuGet_](https://www.nuget.org/packages/Microsoft.Net.Http)

        *   Введение в работу с ним можно найти
            [тут](https://blogs.msdn.microsoft.com/henrikn/2012/02/16/httpclient-is-here/)

        *   Обратите внимание &mdash; он весь построен на асинхронных операциях
            (но мы можем работать в синхронном режиме!)

    *   System.Net.HttpWebRequest &mdash; legacy

*   Работа с HTML:

    *   Можно воспользоваться одной из библиотек, перечисленных
        [тут](https://ru.stackoverflow.com/questions/420354/Как-распарсить-html-в-net/450586)

    *   Самый популярный вариант &mdash;
        [_HtmlAgilityPack_](https://www.nuget.org/packages/HtmlAgilityPack/),
        хотя он достаточно старый и имеет свои проблемы