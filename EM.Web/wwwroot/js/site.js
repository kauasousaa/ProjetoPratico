document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.querySelector("#busca");
  const rows = document.querySelectorAll("table tbody tr");

  if (!rows.length || !searchInput) {
    return;
  }

  const aplicarFiltro = () => {
    const termo = searchInput.value.trim().toLowerCase();
    const tipoBusca = document.querySelector('input[name="tipoBusca"]:checked')?.value || "matricula";

    rows.forEach((row) => {
      const cells = row.cells;
      if (!cells.length) return;

      let textoBusca = "";
      if (tipoBusca === "matricula") {
        textoBusca = cells[0].textContent?.toLowerCase() ?? "";
      } else if (tipoBusca === "nome") {
        textoBusca = cells[1].textContent?.toLowerCase() ?? "";
      }

      const combina = termo ? textoBusca.includes(termo) : true;
      row.classList.toggle("hidden-row", !combina);
    });
  };

  searchInput.addEventListener("keyup", aplicarFiltro);

  document.querySelectorAll('input[name="tipoBusca"]').forEach(radio => {
    radio.addEventListener("change", aplicarFiltro);
  });
});
