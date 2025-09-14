// Configuração do Service Worker para PesquisaX
window.swConfig = {
    version: '1.0.2',
    cacheName: 'pesquisax-v1.0.2',
    staticAssets: [
        '/',
        '/css/app.css',
        '/js/chartInterop.js',
        '/favicon.png',
        '/favicon.svg',
        '/icon-192.png',
        '/icon-512.png',
        '/manifest.json',
        '/offline.html',
        '/_framework/blazor.webassembly.js',
        '/_content/MudBlazor/MudBlazor.min.css',
        '/_content/MudBlazor/MudBlazor.min.js',
        'https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap',
        'https://cdn.jsdelivr.net/npm/chart.js'
    ],
    apiEndpoints: [
        '/api/pesquisas',
        '/api/auth'
    ]
};