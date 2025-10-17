    const apiBaseEl = document.getElementById('apiBase');
    apiBaseEl.textContent = window.APP_CONFIG?.BASE_URL ?? '(sin configurar)';
    const form = document.getElementById('searchForm');
    const qEl = document.getElementById('q');
    const editorialEl = document.getElementById('editorialId');
    const pageSizeEl = document.getElementById('pageSize');
    const rowsEl = document.getElementById('rows');
    const statusEl = document.getElementById('status');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const pageInfo = document.getElementById('pageInfo');

    let page = 1;

    form.addEventListener('submit', async (e) => {
      e.preventDefault();
      page = 1;
      await load();
    });

    prevBtn.addEventListener('click', async () => {
      if (page > 1) { page--; await load(); }
    });

    nextBtn.addEventListener('click', async () => {
      page++; await load();
    });

    async function load() {
      const search = qEl.value.trim();
      const editorialId = editorialEl.value ? Number(editorialEl.value) : undefined;
      const pageSize = Number(pageSizeEl.value) || 10;

      statusEl.textContent = 'Cargando…';
      rowsEl.innerHTML = '';

      try {
        const data = await window.API.fetchLibros({ search, editorialId, page, pageSize });
        // Render filas
        for (const item of data.items) {
          const tr = document.createElement('tr');

          const tdIsbn = document.createElement('td');
          tdIsbn.textContent = item.isbn;

          const tdTitulo = document.createElement('td');
          tdTitulo.textContent = item.titulo;

          const tdEditorial = document.createElement('td');
          tdEditorial.textContent = item.editorial;

          const tdAcciones = document.createElement('td');
          const a = document.createElement('a');
          a.href = `./detail.html?isbn=${encodeURIComponent(item.isbn)}`;
          a.textContent = 'Ver detalle';
          tdAcciones.appendChild(a);

          tr.append(tdIsbn, tdTitulo, tdEditorial, tdAcciones);
          rowsEl.appendChild(tr);
        }

        const totalPages = Math.max(1, Math.ceil(data.total / data.pageSize));
        pageInfo.textContent = `Página ${data.page} de ${totalPages} — ${data.total} total`;
        prevBtn.disabled = page <= 1;
        nextBtn.disabled = page >= totalPages;

        statusEl.textContent = data.items.length ? '' : 'Sin resultados';
      } catch (err) {
        statusEl.textContent = `Error: ${err.message}`;
      }
    }

    // carga inicial
    load();