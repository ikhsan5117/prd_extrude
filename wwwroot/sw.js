const CACHE_NAME = 'velasto-pwa-cache-v3';
const ASSETS_TO_CACHE = [
  // Halaman utama
  '/Account/Login',
  '/Dimensi/Index',
  '/ProductionReport/Create',
  '/Dimensi/History',

  // Lokal aset — CSS
  '/lib/offline-cdn/bootstrap.min.css',
  '/lib/offline-cdn/bootstrap-icons.css',
  '/lib/offline-cdn/sweetalert2.min.css',
  '/lib/offline-cdn/caveat.css',
  '/css/site.css',

  // Lokal aset — JS
  '/lib/offline-cdn/bootstrap.bundle.min.js',
  '/lib/offline-cdn/jquery-3.6.0.min.js',
  '/lib/offline-cdn/sweetalert2.all.min.js',
  '/lib/offline-cdn/html5-qrcode.min.js',
  '/lib/offline-cdn/chart.umd.min.js',
  '/lib/offline-cdn/chartjs-plugin-zoom.min.js',
  '/lib/offline-cdn/hammer.min.js',
  '/lib/offline-cdn/signalr.min.js',
  '/lib/offline-cdn/html2pdf.bundle.min.js',
  '/lib/offline-cdn/jquery.validate.min.js',
  '/lib/offline-cdn/jquery.validate.unobtrusive.min.js',
  '/js/offline-db.js',
  '/js/sync-engine.js',

  // Font
  '/lib/offline-cdn/fonts/bootstrap-icons.woff2',
  '/lib/offline-cdn/fonts/bootstrap-icons.woff',
  '/lib/offline-cdn/fonts/caveat.ttf',
  '/favicon.svg',
  '/images/icon-192.png',
  '/images/icon-512.png',
];

self.addEventListener('install', (event) => {
  self.skipWaiting();
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => cache.addAll(ASSETS_TO_CACHE))
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((cacheNames) =>
      Promise.all(
        cacheNames.map((name) => {
          if (name !== CACHE_NAME) return caches.delete(name);
        })
      )
    )
  );
  self.clients.claim();
});

self.addEventListener('fetch', (event) => {
  if (event.request.method !== 'GET') return;

  // Biarkan API calls & data diurus langsung (offline handled by IndexedDB di App.cshtml)
  const url = event.request.url;
  if (url.includes('/ProductionReport/Get') ||
      url.includes('/Dimensi/Get') ||
      url.includes('/Dimensi/SaveData') ||
      url.includes('/api/')) {
    return;
  }

  const isNavigate = event.request.mode === 'navigate';

  if (isNavigate) {
    // NETWORK FIRST untuk halaman HTML
    event.respondWith(
      fetch(event.request)
        .then((networkResponse) => {
          const clone = networkResponse.clone();
          caches.open(CACHE_NAME).then((cache) => cache.put(event.request, clone));
          return networkResponse;
        })
        .catch(() =>
          caches.match(event.request, { ignoreSearch: true }).then((cached) => {
            if (cached) return cached;
            return caches.match('/Dimensi/Index');
          })
        )
    );
  } else {
    // CACHE FIRST untuk CSS, JS, font, gambar
    event.respondWith(
      caches.match(event.request, { ignoreSearch: true }).then((cached) => {
        if (cached) return cached;
        return fetch(event.request).then((networkResponse) => {
          if (url.startsWith(self.location.origin)) {
            const clone = networkResponse.clone();
            caches.open(CACHE_NAME).then((cache) => cache.put(event.request, clone));
          }
          return networkResponse;
        }).catch(() => { /* ignore offline asset errors */ });
      })
    );
  }
});
