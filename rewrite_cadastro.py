from pathlib import Path

content = """@model Aluno
@using EM.Domain.Enuns
@{
    var isEdicao = (ViewBag.IsEdicao as bool?) ?? false;
    ViewData["Title"] = isEdicao ? "Edição de aluno" : "Cadastrar aluno";
    var tituloPrincipal = isEdicao ? "Editar aluno" : "Cadastrar Aluno";
}

<div class="d-flex justify-content-center align-items-start min-vh-100 py-4">
    <div class="w-100" style="max-width:540px;">
        <div class="card border-0 shadow-sm">
            <div class="card-body px-4 py-5">
                <h1 class="display-6 fw-light text-center mb-2">@tituloPrincipal</h1>
                <p class="text-muted text-center mb-4">Preencha os dados abaixo para salvar o cadastro do aluno.</p>

                <form asp-action="Cadastro" method="post" class="row g-3">
                    @Html.AntiForgeryToken()
                    <input asp-for="Id" type="hidden" />
                    <div asp-validation-summary="ModelOnly" class="alert alert-danger mb-2" role="alert"></div>

                    <div class="col-12">
                        <label asp-for="Matricula" class="form-label small text-uppercase">Matrícula</label>
                        <input asp-for="Matricula" class="form-control form-control-lg" placeholder="Informe a matrícula" />
                        <span asp-validation-for="Matricula" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label asp-for="NomeCompleto" class="form-label small text-uppercase">Nome completo</label>
                        <input asp-for="NomeCompleto" class="form-control form-control-lg" placeholder="Digite o nome completo" />
                        <span asp-validation-for="NomeCompleto" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label asp-for="CPF" class="form-label small text-uppercase">CPF (opcional)</label>
                        <input asp-for="CPF" class="form-control form-control-lg" placeholder="000.000.000-00" />
                        <span asp-validation-for="CPF" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label asp-for="DataNascimento" class="form-label small text-uppercase">Nascimento</label>
                        <input asp-for="DataNascimento" class="form-control form-control-lg" type="date" />
                        <span asp-validation-for="DataNascimento" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label asp-for="Genero" class="form-label small text-uppercase">Sexo</label>
                        <select asp-for="Genero" class="form-select form-select-lg">
                            <option value="">Selecione</option>
                            <option value="@SexoEnum.Masculino">Masculino</option>
                            <option value="@SexoEnum.Feminino">Feminino</option>
                        </select>
                        <span asp-validation-for="Genero" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label class="form-label small text-uppercase">Cidade</label>
                        <select asp-for="Residencia.Id" asp-items="ViewBag.Cidades" class="form-select form-select-lg">
                            <option value="0">Selecione</option>
                        </select>
                        <span asp-validation-for="Residencia.Id" class="text-danger small"></span>
                    </div>

                    <div class="col-12">
                        <label class="form-label small text-uppercase">UF</label>
                        <input class="form-control form-control-lg" value="@Model.Residencia?.Estado" readonly />
                    </div>

                    <div class="col-12 d-flex justify-content-between mt-4">
                        <a asp-controller="Home" asp-action="Index" class="btn text-white px-4" style="background:#5a5a5a;">Voltar</a>
                        <button type="submit" class="btn text-white px-4" style="background:#00b1d6;">Salvar</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
"""

Path("EM.Web/Views/AdministracaoAluno/Cadastro.cshtml").write_text(content, encoding="utf-8")



