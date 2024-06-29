document.addEventListener('DOMContentLoaded', function () {
    var link = document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'icon';
    link.href = '/swagger-ui/favicon.ico';
    document.getElementsByTagName('head')[0].appendChild(link);
});