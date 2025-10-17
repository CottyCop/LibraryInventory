(function () {
  // 1) Lectura de configuración (definida en config.js)
  const BASE = window.APP_CONFIG?.BASE_URL ?? "";
  if (!BASE) {
    console.warn("[API] BASE_URL no configurada en window.APP_CONFIG.BASE_URL");
  }

  // 2) Utilidad: construir querystring a partir de un objeto
  function buildQuery(params) {
    const sp = new URLSearchParams();
    for (const [k, v] of Object.entries(params || {})) {
      if (v !== undefined && v !== null && v !== "") sp.set(k, String(v));
    }
    return sp.toString(); // "a=1&b=xyz"
  }

  // 3) Helper central: fetch con manejo de errores + timeout
  async function getJsonOrThrow(url, init = {}, { timeoutMs = 15000 } = {}) {
    // AbortController para cortar la petición si se cuelga
    const controller = new AbortController();
    const id = setTimeout(() => controller.abort(), timeoutMs);

    try {
      const res = await fetch(url, { ...init, signal: controller.signal });
      if (!res.ok) {
        // Códigos 4xx/5xx: leemos el cuerpo (si llega) para brindar info
        const text = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status}: ${text || res.statusText}`);
      }
      return await res.json();
    } catch (e) {
      // Errores de red, CORS o abort: caen aquí
      if (e.name === "AbortError") {
        throw new Error("La solicitud tardó demasiado (timeout).");
      }
      throw e; // propaga el error para que la UI lo maneje
    } finally {
      clearTimeout(id);
    }
  }

  // 4) Endpoint: listado/paginado/búsqueda de libros
  async function fetchLibros({ search = "", editorialId, page = 1, pageSize = 20 } = {}) {
    const q = buildQuery({ search, editorialId, page, pageSize });
    const url = `${BASE}/Inventario/Libros/TodosLibros?${q}`;
    return await getJsonOrThrow(url);
    // Respuesta esperada: { total, page, pageSize, items: LibroListItemDto[] }
  }

  // 5) Endpoint: detalle por ISBN
  async function fetchLibroPorIsbn(isbn) {
    if (isbn == null || Number.isNaN(Number(isbn))) {
      throw new Error("ISBN inválido.");
    }
    const url = `${BASE}/Inventario/Libros/LibrosPorISBN/${isbn}`;
    try {
      return await getJsonOrThrow(url);
      // Respuesta esperada: LibroDetalleDto
    } catch (e) {
      // Personalizamos 404 para mostrar mensaje más claro en UI
      if (String(e.message).startsWith("HTTP 404")) {
        throw new Error("Libro no encontrado.");
      }
      throw e;
    }
  }

  window.API = {
    fetchLibros,
    fetchLibroPorIsbn,
    
    buildQuery,
  };
})();
