/**
 * Price Matrix Manager
 * Handles interaction and data submission for the dimension-based pricing matrix
 */
class PriceMatrix {
    constructor(config) {
        this.itemId = config.itemId;
        this.saveUrl = config.saveUrl;
        this.antiForgeryToken = this.extractAntiForgeryToken(config.antiForgeryToken);
        this.widthRanges = config.widthRanges || [];
        this.lengthRanges = config.lengthRanges || [];

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

        // Add CSV import listeners
        document.getElementById('importCsvBtn')?.addEventListener('click', () => this.handleImportClick());
        document.getElementById('csvFileInput')?.addEventListener('change', (e) => this.handleFileSelected(e));

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
            // Prepare request payload
            const payload = {
                itemId: this.itemId,
                prices: prices
            };

            const response = await fetch(this.saveUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
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

    /**
     * Handle import button click
     */
    handleImportClick() {
        const fileInput = document.getElementById('csvFileInput');
        if (fileInput) {
            fileInput.click();
        }
    }

    /**
     * Handle file selected event
     */
    async handleFileSelected(event) {
        const file = event.target.files[0];
        if (!file) return;

        // Reset file input
        event.target.value = '';

        try {
            const csvText = await this.readFileAsText(file);
            const csvData = this.parseCSV(csvText);

            if (this.validateCSVDimensions(csvData)) {
                this.loadCSVData(csvData);
                this.showStatus('CSV importado correctamente. Revisa los cambios y haz clic en "Guardar" para confirmar.', 'success');
            }
        } catch (error) {
            console.error('CSV import error:', error);
            this.showStatus(`Error al importar CSV: ${error.message}`, 'danger');
        }
    }

    /**
     * Read file as text
     */
    readFileAsText(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = (e) => resolve(e.target.result);
            reader.onerror = (e) => reject(new Error('Error al leer el archivo'));
            reader.readAsText(file);
        });
    }

    /**
     * Parse CSV text into 2D array
     */
    parseCSV(csvText) {
        const lines = csvText.trim().split('\n');
        const data = [];

        for (let line of lines) {
            const row = [];
            let current = '';
            let inQuotes = false;

            for (let i = 0; i < line.length; i++) {
                const char = line[i];

                if (char === '"') {
                    inQuotes = !inQuotes;
                } else if (char === ',' && !inQuotes) {
                    row.push(current.trim());
                    current = '';
                } else {
                    current += char;
                }
            }
            row.push(current.trim());
            data.push(row);
        }

        return data;
    }

    /**
     * Parse price value (removes $, commas, etc.)
     */
    parsePrice(value) {
        if (!value || value === '') return null;

        // Remove $, quotes, and commas
        const cleaned = value.replace(/[\$",]/g, '').trim();

        if (cleaned === '') return null;

        const num = parseFloat(cleaned);
        return isNaN(num) ? null : num;
    }

    /**
     * Validate CSV dimensions match configured ranges
     */
    validateCSVDimensions(csvData) {
        if (!csvData || csvData.length < 2) {
            throw new Error('El CSV debe tener al menos 2 filas (encabezados + datos)');
        }

        // Extract width values from header row (skip first empty cell)
        const csvWidths = csvData[0].slice(1).map(v => parseFloat(v)).filter(v => !isNaN(v));

        // Extract length values from first column (skip first empty cell)
        const csvLengths = csvData.slice(1).map(row => parseFloat(row[0])).filter(v => !isNaN(v));

        // Validate widths
        if (csvWidths.length !== this.widthRanges.length) {
            throw new Error(`El CSV tiene ${csvWidths.length} columnas de ancho, pero se esperan ${this.widthRanges.length}`);
        }

        // Validate lengths
        if (csvLengths.length !== this.lengthRanges.length) {
            throw new Error(`El CSV tiene ${csvLengths.length} filas de largo, pero se esperan ${this.lengthRanges.length}`);
        }

        // Validate each width matches
        for (let i = 0; i < csvWidths.length; i++) {
            const csvWidth = csvWidths[i];
            const expectedWidth = this.widthRanges[i].Min;

            if (Math.abs(csvWidth - expectedWidth) > 0.01) {
                throw new Error(`El ancho en la columna ${i + 1} (${csvWidth}) no coincide con el esperado (${expectedWidth})`);
            }
        }

        // Validate each length matches
        for (let i = 0; i < csvLengths.length; i++) {
            const csvLength = csvLengths[i];
            const expectedLength = this.lengthRanges[i].Min;

            if (Math.abs(csvLength - expectedLength) > 0.01) {
                throw new Error(`El largo en la fila ${i + 1} (${csvLength}) no coincide con el esperado (${expectedLength})`);
            }
        }

        return true;
    }

    /**
     * Load CSV data into matrix inputs
     */
    loadCSVData(csvData) {
        let loadedCount = 0;

        // Iterate through data rows (skip header row)
        for (let l = 0; l < this.lengthRanges.length && (l + 1) < csvData.length; l++) {
            const row = csvData[l + 1]; // +1 to skip header row

            // Iterate through columns (skip first column which is length value)
            for (let w = 0; w < this.widthRanges.length && (w + 1) < row.length; w++) {
                const key = `${w}_${l}`; // Match the key format from DimensionRanges.CreateRangeKey
                const priceValue = this.parsePrice(row[w + 1]); // +1 to skip first column

                const input = document.querySelector(`input[data-key="${key}"]`);
                if (input) {
                    if (priceValue !== null && priceValue > 0) {
                        input.value = priceValue.toFixed(2);
                    } else {
                        input.value = '';
                    }

                    // Trigger input change event to mark as modified
                    input.dispatchEvent(new Event('input', { bubbles: true }));
                    loadedCount++;
                }
            }
        }

        console.log(`Loaded ${loadedCount} prices from CSV`);
    }
}

// Export for use in view
window.PriceMatrix = PriceMatrix;
