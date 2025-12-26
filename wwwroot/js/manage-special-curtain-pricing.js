// Special Curtain Fabric Matrix (6 heights × 29 widths)
const SPECIAL_CURTAIN_FABRIC_MATRIX = {
    widths: [1.2, 1.4, 1.6, 1.8, 2, 2.2, 2.4, 2.6, 2.8, 3, 3.2, 3.4, 3.6, 3.8, 4, 4.2, 4.4, 4.6, 4.8, 5, 5.2, 5.4, 5.6, 5.8, 6, 6.5, 7, 7.5, 8],
    heights: [1.0, 1.6, 1.8, 2.0, 2.2, 2.4],
    fabricUsage: [
        [1.4, 2.8, 2.8, 2.8, 2.8, 2.8, 2.8, 4.2, 4.2, 4.2, 4.2, 4.2, 4.2, 5.6, 5.6, 5.6, 5.6, 5.6, 5.6, 5.6, 7, 7, 7, 7, 7, 8.4, 8.4, 8.4, 8.4],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2]
    ],

    getFabricUsage(widthIndex, heightIndex) {
        if (heightIndex >= 0 && heightIndex < this.fabricUsage.length &&
            widthIndex >= 0 && widthIndex < this.fabricUsage[heightIndex].length) {
            return this.fabricUsage[heightIndex][widthIndex];
        }
        return 0;
    }
};

class SpecialCurtainPricing {
    constructor(config) {
        this.config = config;
        this.calculatedPrices = null;
        this.init();
    }

    init() {
        // Event listeners
        document.getElementById('calculateBtn')?.addEventListener('click', () => this.calculatePrices());
        document.getElementById('saveBtn')?.addEventListener('click', () => this.savePricing());

        // Quick paste event listeners
        document.getElementById('applyPasteBtn')?.addEventListener('click', () => this.applyPastedValues());
        document.getElementById('clearPasteBtn')?.addEventListener('click', () => this.clearPasteArea());

        // Monitor input changes
        document.querySelectorAll('.profit-margin-input').forEach(input => {
            input.addEventListener('input', () => {
                input.classList.add('modified');
                this.calculatedPrices = null; // Reset calculated prices when inputs change
                document.getElementById('saveBtn').disabled = true;
            });
        });

        document.getElementById('basePrice')?.addEventListener('input', () => {
            this.calculatedPrices = null;
            document.getElementById('saveBtn').disabled = true;
        });

        document.getElementById('taxPercent')?.addEventListener('input', () => {
            this.calculatedPrices = null;
            document.getElementById('saveBtn').disabled = true;
        });
    }

    applyPastedValues() {
        const textarea = document.getElementById('quickPasteTextarea');
        const statusDiv = document.getElementById('pasteStatus');
        const text = textarea.value.trim();

        if (!text) {
            statusDiv.innerHTML = '<div class="alert alert-warning"><i class="bi bi-exclamation-triangle"></i> Por favor pegue los valores primero</div>';
            return;
        }

        // Parse the pasted text
        const lines = text.split(/\r?\n/);
        const values = [];
        const errors = [];

        lines.forEach((line, index) => {
            const trimmed = line.trim();
            if (!trimmed) return; // Skip empty lines

            // Remove % sign if present and parse number
            const cleanValue = trimmed.replace('%', '').trim();
            const numValue = parseFloat(cleanValue);

            if (isNaN(numValue)) {
                errors.push(`Línea ${index + 1}: "${line}" no es un número válido`);
            } else if (numValue < 0) {
                errors.push(`Línea ${index + 1}: ${numValue} es negativo`);
            } else if (numValue > 1000) {
                errors.push(`Línea ${index + 1}: ${numValue}% parece muy alto`);
            } else {
                values.push(numValue);
            }
        });

        // Show errors if any
        if (errors.length > 0) {
            const errorHtml = '<div class="alert alert-danger">' +
                '<i class="bi bi-x-circle"></i> <strong>Errores encontrados:</strong><br>' +
                '<small>' + errors.slice(0, 5).join('<br>') + '</small>' +
                (errors.length > 5 ? '<br><small>...y ' + (errors.length - 5) + ' más</small>' : '') +
                '</div>';
            statusDiv.innerHTML = errorHtml;
            return;
        }

        if (values.length === 0) {
            statusDiv.innerHTML = '<div class="alert alert-warning"><i class="bi bi-exclamation-triangle"></i> No se encontraron valores válidos</div>';
            return;
        }

        // Apply values to inputs
        const profitInputs = document.querySelectorAll('.profit-margin-input');
        let appliedCount = 0;

        profitInputs.forEach((input, index) => {
            if (index < values.length) {
                input.value = values[index];
                input.classList.add('paste-success', 'modified');

                // Remove animation class after animation completes
                setTimeout(() => {
                    input.classList.remove('paste-success');
                }, 600);

                appliedCount++;
            }
        });

        // Show success message
        const totalInputs = profitInputs.length;
        let message = `<div class="alert alert-success">
            <i class="bi bi-check-circle-fill"></i>
            <strong>${appliedCount} valores aplicados correctamente</strong>`;

        if (appliedCount < totalInputs) {
            message += `<br><small>Nota: Se aplicaron ${appliedCount} de ${totalInputs} valores posibles</small>`;
        }

        if (values.length > totalInputs) {
            message += `<br><small>Se ignoraron ${values.length - totalInputs} valores extras</small>`;
        }

        message += '</div>';
        statusDiv.innerHTML = message;

        // Clear textarea after successful application
        textarea.value = '';

        // Reset calculated prices since inputs changed
        this.calculatedPrices = null;
        document.getElementById('saveBtn').disabled = true;

        // Auto-hide success message after 5 seconds
        setTimeout(() => {
            statusDiv.innerHTML = '';
        }, 5000);
    }

    clearPasteArea() {
        const textarea = document.getElementById('quickPasteTextarea');
        const statusDiv = document.getElementById('pasteStatus');

        textarea.value = '';
        statusDiv.innerHTML = '';
        textarea.focus();
    }

    calculatePrices() {
        const basePrice = parseFloat(document.getElementById('basePrice').value);
        const taxPercent = parseFloat(document.getElementById('taxPercent').value);

        if (!basePrice || basePrice <= 0) {
            alert('Por favor ingrese un precio base válido');
            return;
        }

        if (taxPercent === null || taxPercent === undefined || taxPercent < 0) {
            alert('Por favor ingrese un IVA válido');
            return;
        }

        // Collect profit margins
        const profitMargins = {};
        const profitInputs = document.querySelectorAll('.profit-margin-input');
        let hasError = false;

        profitInputs.forEach(input => {
            const heightIndex = input.dataset.heightIndex;
            const value = parseFloat(input.value) || 0;

            if (value < 0) {
                hasError = true;
                input.classList.add('is-invalid');
            } else {
                input.classList.remove('is-invalid');
                profitMargins[heightIndex] = value;
            }
        });

        if (hasError) {
            alert('Por favor corrija los valores de utilidad inválidos');
            return;
        }

        // Calculate all prices
        this.calculatedPrices = {};

        for (let h = 0; h < this.config.lengthRanges.length; h++) {
            const profitMargin = profitMargins[h] || 0;

            for (let w = 0; w < this.config.widthRanges.length; w++) {
                const fabricUsage = SPECIAL_CURTAIN_FABRIC_MATRIX.getFabricUsage(w, h);

                // Formula: (BasePrice * FabricUsage) * ((ProfitMargin * 0.01) + 1) * ((Tax * 0.01) + 1)
                const price = (basePrice * fabricUsage)
                    * ((profitMargin * 0.01) + 1)
                    * ((taxPercent * 0.01) + 1);

                const key = `${w}_${h}`;
                this.calculatedPrices[key] = Math.round(price * 100) / 100; // Round to 2 decimals
            }
        }

        // Show preview
        this.showPreview();

        // Enable save button
        document.getElementById('saveBtn').disabled = false;
    }

    showPreview() {
        if (!this.calculatedPrices) {
            return;
        }

        const previewContainer = document.getElementById('previewContainer');

        let html = '<div class="preview-scroll-container">';
        html += '<table class="table table-sm preview-matrix">';

        // Header
        html += '<thead><tr>';
        html += '<th class="sticky-corner">Ancho → Largo ↓</th>';
        this.config.widthRanges.forEach(range => {
            html += `<th title="${range.Min} - ${range.Max}">${range.Min}<br/>${range.Max}</th>`;
        });
        html += '</tr></thead>';

        // Body
        html += '<tbody>';
        for (let h = 0; h < this.config.lengthRanges.length; h++) {
            const heightRange = this.config.lengthRanges[h];
            html += '<tr>';
            html += `<th>${heightRange.Min} - ${heightRange.Max}</th>`;

            for (let w = 0; w < this.config.widthRanges.length; w++) {
                const key = `${w}_${h}`;
                const price = this.calculatedPrices[key] || 0;
                html += `<td class="calculated-price">$${price.toFixed(2)}</td>`;
            }

            html += '</tr>';
        }
        html += '</tbody>';

        html += '</table>';
        html += '</div>';

        previewContainer.innerHTML = html;
    }

    async savePricing() {
        if (!this.calculatedPrices) {
            alert('Primero debe calcular los precios');
            return;
        }

        const basePrice = parseFloat(document.getElementById('basePrice').value);
        const taxPercent = parseFloat(document.getElementById('taxPercent').value);

        // Collect profit margins
        const profitMargins = {};
        document.querySelectorAll('.profit-margin-input').forEach(input => {
            const heightIndex = input.dataset.heightIndex;
            const value = parseFloat(input.value) || 0;
            profitMargins[heightIndex] = value;
        });

        const data = {
            itemId: this.config.itemId,
            basePrice: basePrice,
            taxPercent: taxPercent,
            profitMargins: profitMargins
        };

        try {
            // Show loading
            const saveBtn = document.getElementById('saveBtn');
            const originalText = saveBtn.innerHTML;
            saveBtn.disabled = true;
            saveBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando...';

            const response = await fetch(this.config.saveUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (result.success) {
                document.getElementById('saveStatus').innerHTML =
                    '<div class="alert alert-success mt-2">' +
                    '<i class="bi bi-check-circle-fill"></i> ' + result.message +
                    '</div>';

                // Clear modified flags
                document.querySelectorAll('.profit-margin-input').forEach(input => {
                    input.classList.remove('modified');
                });

                // NO REDIRECT - just show success message and stay on the page
                saveBtn.innerHTML = '<i class="bi bi-check-circle-fill"></i> Guardado exitosamente';
                saveBtn.classList.remove('btn-primary');
                saveBtn.classList.add('btn-success');
            } else {
                document.getElementById('saveStatus').innerHTML =
                    '<div class="alert alert-danger mt-2">' +
                    '<i class="bi bi-exclamation-triangle-fill"></i> ' + result.message +
                    '</div>';
                saveBtn.disabled = false;
                saveBtn.innerHTML = originalText;
            }
        } catch (error) {
            console.error('Error saving pricing:', error);
            document.getElementById('saveStatus').innerHTML =
                '<div class="alert alert-danger mt-2">' +
                '<i class="bi bi-exclamation-triangle-fill"></i> Error al guardar: ' + error.message +
                '</div>';

            const saveBtn = document.getElementById('saveBtn');
            saveBtn.disabled = false;
            saveBtn.innerHTML = originalText;
        }
    }

    getAntiForgeryToken() {
        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenInput ? tokenInput.value : '';
    }
}
