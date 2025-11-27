document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.querySelector("#busca");
  const filterSelect = document.querySelector("#filtro");
  const clearButton = document.querySelector("#limparFiltros");
  const rows = document.querySelectorAll("table tbody tr");
  const totalLabel = document.querySelector("#totalAlunos");
  const femLabel = document.querySelector("#alunosFemininos");
  const mascLabel = document.querySelector("#alunosMasculinos");
  const percFeminino = document.querySelector("#percFeminino");
  const percMasculino = document.querySelector("#percMasculino");
  const barraFeminino = document.querySelector("#barraFeminino");
  const barraMasculino = document.querySelector("#barraMasculino");

  if (!rows.length) {
    return;
  }

  const getGenero = (row) => row.cells[2].textContent?.trim().toLowerCase();

  const atualizarEstatisticas = () => {
    let visiveis = 0;
    let femininos = 0;
    let masculinos = 0;

    rows.forEach((row) => {
      if (row.classList.contains("hidden-row")) {
        return;
      }

      visiveis += 1;
      const genero = getGenero(row);
      if (genero === "feminino") {
        femininos += 1;
      } else if (genero === "masculino") {
        masculinos += 1;
      }
    });

    totalLabel.textContent = visiveis.toString();
    femLabel.textContent = femininos.toString();
    mascLabel.textContent = masculinos.toString();

    const porcentagem = (valor) => (visiveis ? Math.round((valor / visiveis) * 100) : 0);
    const femPercent = porcentagem(femininos);
    const mascPercent = porcentagem(masculinos);

    if (percFeminino) {
      percFeminino.textContent = `${femPercent}%`;
    }
    if (percMasculino) {
      percMasculino.textContent = `${mascPercent}%`;
    }

    if (barraFeminino) {
      barraFeminino.style.width = `${femPercent}%`;
    }

    if (barraMasculino) {
      barraMasculino.style.width = `${mascPercent}%`;
    }
  };

  const aplicarFiltro = () => {
    const termo = searchInput?.value.trim().toLowerCase() ?? "";
    const isFiltroAtivo = termo.length > 0;

    rows.forEach((row) => {
      const texto = row.textContent?.toLowerCase() ?? "";
      const combina = termo ? texto.includes(termo) : true;

      row.classList.toggle("hidden-row", !combina);
      row.classList.toggle("row-highlight", combina && isFiltroAtivo);
    });

    atualizarEstatisticas();
  };

  const resetFiltros = () => {
    if (searchInput) {
      searchInput.value = "";
    }
    rows.forEach((row) => {
      row.classList.remove("hidden-row", "row-highlight");
    });
    atualizarEstatisticas();
  };

  searchInput?.addEventListener("keyup", () => {
    aplicarFiltro();
  });

  filterSelect?.addEventListener("change", () => {
    aplicarFiltro();
  });

  clearButton?.addEventListener("click", resetFiltros);

  // Inicializa os contadores ao carregar a página.
  atualizarEstatisticas();
});
