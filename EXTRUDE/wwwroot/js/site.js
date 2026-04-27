// Velasto Production System - Custom JavaScript

// Form validation enhancement
document.addEventListener('DOMContentLoaded', function() {
    // Auto-format decimal inputs
    const decimalInputs = document.querySelectorAll('input[type="number"][step]');
    decimalInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value) {
                const step = parseFloat(this.getAttribute('step'));
                const decimals = step.toString().split('.')[1]?.length || 0;
                this.value = parseFloat(this.value).toFixed(decimals);
            }
        });
    });
    
    // Confirm delete actions
    const deleteButtons = document.querySelectorAll('[data-action="delete"]');
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            if (!confirm('Are you sure you want to delete this item?')) {
                e.preventDefault();
            }
        });
    });
});

// Print functionality
function printPage() {
    window.print();
}

// Auto-dismiss alerts
setTimeout(function() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        const bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    });
}, 5000);
