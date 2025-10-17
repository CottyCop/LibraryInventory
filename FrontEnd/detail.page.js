    const apiBaseEl = document.getElementById('apiBase');
    apiBaseEl.textContent = window.APP_CONFIG?.BASE_URL ?? '(sin configurar)';

    const isbnInput = document.getElementById('isbn');
    const btn = document.getElementById('btn');
    const statusEl = document.getElementById('status');
    const card = document.getElementById('card');
    const titulo = document.getElementById('titulo');
    const isbnText = document.getElementById('isbnText');
    const editorial = document.getElementById('editorial');
    const paginas = document.getElementById('paginas');
    const sinopsis = document.getElementById('sinopsis');
    const autores = document.getElementById('autores');

    // si viene ?isbn=123 en la URL, auto-carga
    const url = new URL(location.href);
    const initialIsbn = url.searchParams.get('isbn');
    if (initialIsbn) { isbnInput.value = initialIsbn; buscar(); }

    btn.addEventListener('click', buscar);

    async function buscar() {
      const isbn = isbnInput.value.trim();
      if (!isbn) { statusEl.textContent = 'Ingresa un ISBN.'; return; }

      statusEl.textContent = 'Buscando…';
      card.hidden = true;
      autores.innerHTML = '';

      try {
        const dto = await window.API.fetchLibroPorIsbn(isbn);
        titulo.textContent = dto.titulo;
        isbnText.textContent = dto.isbn;
        editorial.textContent = dto.editorial;
        paginas.textContent = dto.n_paginas;
        sinopsis.textContent = dto.sipnosis;
        dto.autores.forEach(n => {
          const li = document.createElement('li');
          li.textContent = n;
          autores.appendChild(li);
        });
        card.hidden = false;
        statusEl.textContent = '';
      } catch (err) {
        statusEl.textContent = `Error: ${err.message}`;
      }
    }