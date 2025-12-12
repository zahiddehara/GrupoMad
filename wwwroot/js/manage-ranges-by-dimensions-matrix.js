/**
 * Price Matrix Manager
 * Handles interaction and data submission for the dimension-based pricing matrix
 */
class PriceMatrix {
    constructor(config) {
        this.itemId = config.itemId;
        this.saveUrl = config.saveUrl;
        this.antiForgeryToken = this.extractAntiForgeryToken(config.antiForgeryToken);

        this.inputs = document.querySelectorAll('.price-input');
        this.changedCells = new Map(); // key -> value
        this.originalValues = new Map(); // key -> original value

        this.init();
    }

    /**
     * Extract token value from HTML string
     */
    extractAntiForgeryToken(htmlString) {
        const div = document.createElement('div');
        div.innerHTML = htmlString;
        const input = div.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : '';
    }

    /**
     * Initialize event listeners and store original values
     */
    init() {
        // Store original values
        this.inputs.forEach(input => {
            const key = input.dataset.key;
            const value = input.value.trim();
            this.originalValues.set(key, value);
        });

        // Add input event listeners
        this.inputs.forEach(input => {
            input.addEventListener('input', (e) => this.handleInputChange(e));
            input.addEventListener('blur', (e) => this.validateInput(e));
        });

        // Add save button listeners
        document.getElementById('saveAllBtn')?.addEventListener('click', () => this.saveChanges());
        document.getElementById('saveAllBtn2')?.addEventListener('click', () => this.saveChanges());

        // Warn before leaving if there are unsaved changes
        window.addEventListener('beforeunload', (e) => {
            if (this.changedCells.size > 0) {
                e.preventDefault();
                e.returnValue = 'Tienes cambios sin guardar. ¿Estás seguro de que quieres salir?';
                return e.returnValue;
            }
        });

        this.updateChangeCounter();
    }

    /**
     * Handle input change event
     */
    handleInputChange(event) {
        const input = event.target;
        const key = input.dataset.key;
        const newValue = input.value.trim();
        const originalValue = this.originalValues.get(key);

        // Check if value changed from original
        if (newValue !== originalValue) {
            this.changedCells.set(key, newValue);
            input.parentElement.classList.add('modified');
        } else {
            this.changedCells.delete(key);
            input.parentElement.classList.remove('modified');
        }

        this.updateChangeCounter();
    }

    /**
     * Validate input value (only positive numbers)
     */
    validateInput(event) {
        const input = event.target;
        const value = parseFloat(input.value);

        if (input.value.trim() !== '' && (isNaN(value) || value < 0)) {
            input.classList.add('is-invalid');
            input.value = this.originalValues.get(input.dataset.key) || '';
        } else {
            input.classList.remove('is-invalid');
        }
    }

    /**
     * Update change counter display
     */
    updateChangeCounter() {
        const count = this.changedCells.size;
        const counterText = count > 0
            ? `${count} celda${count !== 1 ? 's' : ''} modificada${count !== 1 ? 's' : ''}`
            : 'Sin cambios';

        const counter1 = document.getElementById('changeCounter');
        const counter2 = document.getElementById('changeCounter2');

        if (counter1) counter1.textContent = counterText;
        if (counter2) counter2.textContent = counterText;

        // Enable/disable save buttons
        const saveBtn1 = document.getElementById('saveAllBtn');
        const saveBtn2 = document.getElementById('saveAllBtn2');

        if (saveBtn1) saveBtn1.disabled = count === 0;
        if (saveBtn2) saveBtn2.disabled = count === 0;
    }

    /**
     * Save changes to server via AJAX
     */
    async saveChanges() {
        if (this.changedCells.size === 0) {
            this.showStatus('No hay cambios para guardar', 'info');
            return;
        }

        // Build prices object (only include valid positive values)
        const prices = {};
        this.changedCells.forEach((value, key) => {
            const numValue = parseFloat(value);
            if (!isNaN(numValue) && numValue > 0) {
                prices[key] = numValue;
            }
        });

        // Show loading state
        const saveBtn1 = document.getElementById('saveAllBtn');
        const saveBtn2 = document.getElementById('saveAllBtn2');
        const originalText1 = saveBtn1?.innerHTML;
        const originalText2 = saveBtn2?.innerHTML;

        if (saveBtn1) {
            saveBtn1.disabled = true;
            saveBtn1.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Guardando...';
        }
        if (saveBtn2) {
            saveBtn2.disabled = true;
            saveBtn2.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Guardando...';
        }

        try {
            const formData = new FormData();
            formData.append('itemId', this.itemId);
            formData.append('__RequestVerificationToken', this.antiForgeryToken);

            // Add each price as separate form field
            Object.keys(prices).forEach(key => {
                formData.append(`prices[${key}]`, prices[key]);
            });

            const response = await fetch(this.saveUrl, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                this.showStatus(result.message, 'success');

                // Update original values and clear changed cells
                this.changedCells.forEach((value, key) => {
                    this.originalValues.set(key, value);
                    const input = document.querySelector(`input[data-key="${key}"]`);
                    if (input) {
                        input.parentElement.classList.remove('modified');
                    }
                });

                this.changedCells.clear();
                this.updateChangeCounter();
            } else {
                this.showStatus(result.message || 'Error al guardar cambios', 'danger');
            }
        } catch (error) {
            console.error('Save error:', error);
            this.showStatus('Error de red. Por favor intenta de nuevo.', 'danger');
        } finally {
            // Restore button state
            if (saveBtn1) {
                saveBtn1.disabled = false;
                saveBtn1.innerHTML = originalText1;
            }
            if (saveBtn2) {
                saveBtn2.disabled = false;
                saveBtn2.innerHTML = originalText2;
            }
        }
    }

    /**
     * Show status message
     */
    showStatus(message, type = 'info') {
        const statusDiv = document.getElementById('saveStatus');
        if (!statusDiv) return;

        const alertClass = `alert alert-${type} alert-dismissible fade show`;
        statusDiv.innerHTML = `
            <div class="${alertClass}" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            const alert = statusDiv.querySelector('.alert');
            if (alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }
        }, 5000);
    }
}

// Export for use in view
window.PriceMatrix = PriceMatrix;
