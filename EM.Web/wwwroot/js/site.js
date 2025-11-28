// JavaScript básico para o sistema

// Aguarda o carregamento completo da página
document.addEventListener('DOMContentLoaded', function() {
    // Inicializa tooltips do Bootstrap se disponível
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Adiciona feedback visual em botões ao clicar
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!this.classList.contains('btn-link')) {
                this.style.transform = 'scale(0.98)';
                setTimeout(() => {
                    this.style.transform = '';
                }, 150);
            }
        });
    });

    // Validação de formulários
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const requiredFields = form.querySelectorAll('[required]');
            let isValid = true;

            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    isValid = false;
                    field.classList.add('is-invalid');
                } else {
                    field.classList.remove('is-invalid');
                }
            });

            if (!isValid) {
                e.preventDefault();
                alert('Por favor, preencha todos os campos obrigatórios.');
            }
        });
    });

    // Remove classes de erro quando o usuário começa a digitar
    const inputs = document.querySelectorAll('input, select, textarea');
    inputs.forEach(input => {
        input.addEventListener('input', function() {
            this.classList.remove('is-invalid');
        });
    });

    // Auto-focus no primeiro campo de busca
    const searchInput = document.getElementById('busca');
    if (searchInput && !searchInput.value) {
        // Não foca automaticamente para não incomodar o usuário
    }
});

// Função para formatar CPF (usada no formulário de aluno)
function formatarCPF(input) {
    let valor = input.value.replace(/\D/g, '');
    
    if (valor.length <= 11) {
        valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
        valor = valor.replace(/(\d{3})(\d)/, '$1.$2');
        valor = valor.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    }
    
    input.value = valor;
}

// Função para confirmar exclusão
function confirmarExclusao(id, nome) {
    if (confirm('Tem certeza que deseja excluir "' + nome + '"?')) {
        const form = document.getElementById('formExcluir');
        if (form) {
            document.getElementById('idExcluir').value = id;
            form.submit();
        }
    }
}

// Função para mostrar mensagens de sucesso/erro (se necessário)
function mostrarMensagem(mensagem, tipo = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${tipo} alert-dismissible fade show`;
    alertDiv.setAttribute('role', 'alert');
    alertDiv.innerHTML = `
        ${mensagem}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    const container = document.querySelector('.container');
    if (container) {
        container.insertBefore(alertDiv, container.firstChild);
        
        // Remove automaticamente após 5 segundos
        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }
}
