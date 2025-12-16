// Curtain Fabric Matrix (converted from CSV)
const CURTAIN_FABRIC_MATRIX = {
    widths: [1.2, 1.4, 1.6, 1.8, 2, 2.2, 2.4, 2.6, 2.8, 3, 3.2, 3.4, 3.6, 3.8, 4, 4.2, 4.4, 4.6, 4.8, 5, 5.2, 5.4, 5.6, 5.8, 6, 6.5, 7, 7.5, 8],
    heights: [1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 3, 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 4, 4.1, 4.2, 4.3, 4.4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9],
    fabricUsage: [
        [1.4, 2.8, 2.8, 2.8, 2.8, 2.8, 2.8, 4.2, 4.2, 4.2, 4.2, 4.2, 4.2, 5.6, 5.6, 5.6, 5.6, 5.6, 5.6, 5.6, 7, 7, 7, 7, 7, 8.4, 8.4, 8.4, 8.4],
        [1.5, 3, 3, 3, 3, 3, 3, 4.5, 4.5, 4.5, 4.5, 4.5, 4.5, 6, 6, 6, 6, 6, 6, 6, 7.5, 7.5, 7.5, 7.5, 7.5, 9, 9, 9, 9],
        [1.6, 3.2, 3.2, 3.2, 3.2, 3.2, 3.2, 4.8, 4.8, 4.8, 4.8, 4.8, 4.8, 6.4, 6.4, 6.4, 6.4, 6.4, 6.4, 6.4, 8, 8, 8, 8, 8, 9.6, 9.6, 9.6, 9.6],
        [1.7, 3.4, 3.4, 3.4, 3.4, 3.4, 3.4, 5.1, 5.1, 5.1, 5.1, 5.1, 5.1, 6.8, 6.8, 6.8, 6.8, 6.8, 6.8, 6.8, 8.5, 8.5, 8.5, 8.5, 8.5, 10.2, 10.2, 10.2, 10.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [2.9, 3.4, 3.9, 4.32, 4.8, 5.3, 5.8, 6.24, 6.72, 7.2, 7.7, 8.2, 8.7, 9.12, 9.6, 10.1, 10.6, 11.04, 11.52, 12, 12.5, 12.96, 13.44, 13.92, 14.4, 15.6, 16.8, 18, 19.2],
        [3, 4.5, 5.2, 5.4, 5.6, 5.8, 6, 9, 9, 9, 9, 9, 9, 12, 12, 12, 12, 12, 12, 12, 15, 15, 15, 15, 15, 18, 18, 18, 18],
        [3.1, 5.1, 6.2, 6.2, 6.2, 6.2, 6.2, 9.3, 9.3, 9.3, 9.3, 9.3, 9.3, 12.4, 12.4, 12.4, 12.4, 12.4, 12.4, 12.4, 15.5, 15.5, 15.5, 15.5, 15.5, 18.6, 18.6, 18.6, 18.6],
        [3.2, 5.2, 6.4, 6.4, 6.4, 6.4, 6.4, 9.6, 9.6, 9.6, 9.6, 9.6, 9.6, 12.8, 12.8, 12.8, 12.8, 12.8, 12.8, 12.8, 16, 16, 16, 16, 16, 19.2, 19.2, 19.2, 19.2],
        [3.3, 5.3, 6.6, 6.6, 6.6, 6.6, 6.6, 9.9, 9.9, 9.9, 9.9, 9.9, 9.9, 13.2, 13.2, 13.2, 13.2, 13.2, 13.2, 13.2, 16.5, 16.5, 16.5, 16.5, 16.5, 19.8, 19.8, 19.8, 19.8],
        [3.4, 5.4, 6.8, 6.8, 6.8, 6.8, 6.8, 10.2, 10.2, 10.2, 10.2, 10.2, 10.2, 13.6, 13.6, 13.6, 13.6, 13.6, 13.6, 13.6, 17, 17, 17, 17, 17, 20.4, 20.4, 20.4, 20.4],
        [3.5, 5.5, 7, 7, 7, 7, 7, 10.5, 10.5, 10.5, 10.5, 10.5, 10.5, 14, 14, 14, 14, 14, 14, 14, 17.5, 17.5, 17.5, 17.5, 17.5, 21, 21, 21, 21],
        [3.6, 6, 7.2, 7.2, 7.2, 7.2, 7.2, 10.8, 10.8, 10.8, 10.8, 10.8, 10.8, 14.4, 14.4, 14.4, 14.4, 14.4, 14.4, 14.4, 18, 18, 18, 18, 18, 21.6, 21.6, 21.6, 21.6],
        [3.7, 6.1, 7.4, 7.4, 7.4, 7.4, 7.4, 11.1, 11.1, 11.1, 11.1, 11.1, 11.1, 14.8, 14.8, 14.8, 14.8, 14.8, 14.8, 14.8, 18.5, 18.5, 18.5, 18.5, 18.5, 22.2, 22.2, 22.2, 22.2],
        [3.8, 6.2, 7.6, 7.6, 7.6, 7.6, 7.6, 11.4, 11.4, 11.4, 11.4, 11.4, 11.4, 15.2, 15.2, 15.2, 15.2, 15.2, 15.2, 15.2, 19, 19, 19, 19, 19, 22.8, 22.8, 22.8, 22.8],
        [3.9, 6.3, 7.8, 7.8, 7.8, 7.8, 7.8, 11.7, 11.7, 11.7, 11.7, 11.7, 11.7, 15.6, 15.6, 15.6, 15.6, 15.6, 15.6, 15.6, 19.5, 19.5, 19.5, 19.5, 19.5, 23.4, 23.4, 23.4, 23.4],
        [4, 7, 8, 8, 8, 8, 8, 12, 12, 12, 12, 12, 12, 16, 16, 16, 16, 16, 16, 16, 20, 20, 20, 20, 20, 24, 24, 24, 24],
        [4.1, 7.2, 8.2, 8.2, 8.2, 8.2, 8.2, 12.3, 12.3, 12.3, 12.3, 12.3, 12.3, 16.4, 16.4, 16.4, 16.4, 16.4, 16.4, 16.4, 20.5, 20.5, 20.5, 20.5, 20.5, 24.6, 24.6, 24.6, 24.6],
        [4.2, 7.6, 8.4, 8.4, 8.4, 8.4, 8.4, 12.6, 12.6, 12.6, 12.6, 12.6, 12.6, 16.8, 16.8, 16.8, 16.8, 16.8, 16.8, 16.8, 21, 21, 21, 21, 21, 25.2, 25.2, 25.2, 25.2],
        [4.3, 7.8, 8.6, 8.6, 8.6, 8.6, 8.6, 12.9, 12.9, 12.9, 12.9, 12.9, 12.9, 17.2, 17.2, 17.2, 17.2, 17.2, 17.2, 17.2, 21.5, 21.5, 21.5, 21.5, 21.5, 25.8, 25.8, 25.8, 25.8],
        [4.4, 8, 8.8, 8.8, 8.8, 8.8, 8.8, 13.2, 13.2, 13.2, 13.2, 13.2, 13.2, 17.6, 17.6, 17.6, 17.6, 17.6, 17.6, 17.6, 22, 22, 22, 22, 22, 26.4, 26.4, 26.4, 26.4],
        [4.5, 8.2, 9, 9, 9, 9, 9, 13.5, 13.5, 13.5, 13.5, 13.5, 13.5, 18, 18, 18, 18, 18, 18, 18, 22.5, 22.5, 22.5, 22.5, 22.5, 27, 27, 27, 27],
        [4.6, 8.4, 9.2, 9.2, 9.2, 9.2, 9.2, 13.8, 13.8, 13.8, 13.8, 13.8, 13.8, 18.4, 18.4, 18.4, 18.4, 18.4, 18.4, 18.4, 23, 23, 23, 23, 23, 27.6, 27.6, 27.6, 27.6],
        [4.7, 8.6, 9.4, 9.4, 9.4, 9.4, 9.4, 14.1, 14.1, 14.1, 14.1, 14.1, 14.1, 18.8, 18.8, 18.8, 18.8, 18.8, 18.8, 18.8, 23.5, 23.5, 23.5, 23.5, 23.5, 28.2, 28.2, 28.2, 28.2],
        [4.8, 8.8, 9.6, 9.6, 9.6, 9.6, 9.6, 14.4, 14.4, 14.4, 14.4, 14.4, 14.4, 19.2, 19.2, 19.2, 19.2, 19.2, 19.2, 19.2, 24, 24, 24, 24, 24, 28.8, 28.8, 28.8, 28.8],
        [4.9, 9, 9.8, 9.8, 9.8, 9.8, 9.8, 14.7, 14.7, 14.7, 14.7, 14.7, 14.7, 19.6, 19.6, 19.6, 19.6, 19.6, 19.6, 19.6, 24.5, 24.5, 24.5, 24.5, 24.5, 29.4, 29.4, 29.4, 29.4],
        [5.4, 10.8, 10.8, 10.8, 10.8, 10.8, 10.8, 16.2, 16.2, 16.2, 16.2, 16.2, 16.2, 21.6, 21.6, 21.6, 21.6, 21.6, 21.6, 21.6, 27, 27, 27, 27, 27, 32.4, 32.4, 32.4, 32.4],
        [5.9, 11.8, 11.8, 11.8, 11.8, 11.8, 11.8, 17.2, 17.2, 17.2, 17.2, 17.2, 17.2, 23.6, 23.6, 23.6, 23.6, 23.6, 23.6, 23.6, 29.5, 29.5, 29.5, 29.5, 29.5, 35.4, 35.4, 35.4, 35.4],
        [6.4, 12.8, 12.8, 12.8, 12.8, 12.8, 12.8, 19.2, 19.2, 19.2, 19.2, 19.2, 19.2, 25.6, 25.6, 25.6, 25.6, 25.6, 25.6, 25.6, 32, 32, 32, 32, 32, 38.4, 38.4, 38.4, 38.4],
        [6.9, 13.8, 13.8, 13.8, 13.8, 13.8, 13.8, 20.7, 20.7, 20.7, 20.7, 20.7, 20.7, 27.6, 27.6, 27.6, 27.6, 27.6, 27.6, 27.6, 34.5, 34.5, 34.5, 34.5, 34.5, 41.4, 41.4, 41.4, 41.4],
        [7.4, 14.8, 14.8, 14.8, 14.8, 14.8, 14.8, 22.2, 22.2, 22.2, 22.2, 22.2, 22.2, 29.6, 29.6, 29.6, 29.6, 29.6, 29.6, 29.6, 37, 37, 37, 37, 37, 44.4, 44.4, 44.4, 44.4],
        [7.9, 15.8, 15.8, 15.8, 15.8, 15.8, 15.8, 23.7, 23.7, 23.7, 23.7, 23.7, 23.7, 31.6, 31.6, 31.6, 31.6, 31.6, 31.6, 31.6, 39.5, 39.5, 39.5, 39.5, 39.5, 47.4, 47.4, 47.4, 47.4],
        [8.4, 16.8, 16.8, 16.8, 16.8, 16.8, 16.8, 25.2, 25.2, 25.2, 25.2, 25.2, 25.2, 35.6, 35.6, 35.6, 35.6, 35.6, 35.6, 35.6, 42, 42, 42, 42, 42, 50.4, 50.4, 50.4, 50.4],
        [8.9, 17.8, 17.8, 17.8, 17.8, 17.8, 17.8, 26.7, 26.7, 26.7, 26.7, 26.7, 26.7, 33.6, 33.6, 33.6, 33.6, 33.6, 33.6, 33.6, 44.5, 44.5, 44.5, 44.5, 44.5, 53.4, 53.4, 53.4, 53.4],
        [9.4, 18.8, 18.8, 18.8, 18.8, 18.8, 18.8, 28.2, 28.2, 28.2, 28.2, 28.2, 28.2, 37.6, 37.6, 37.6, 37.6, 37.6, 37.6, 37.6, 47, 47, 47, 47, 47, 56.4, 56.4, 56.4, 56.4]
    ],

    getFabricUsage(widthIndex, heightIndex) {
        if (heightIndex >= 0 && heightIndex < this.fabricUsage.length &&
            widthIndex >= 0 && widthIndex < this.fabricUsage[heightIndex].length) {
            return this.fabricUsage[heightIndex][widthIndex];
        }
        return 0;
    }
};

class CurtainPricing {
    constructor(config) {
        this.config = config;
        this.calculatedPrices = null;
        this.init();
    }

    init() {
        // Event listeners
        document.getElementById('calculateBtn')?.addEventListener('click', () => this.calculatePrices());
        document.getElementById('saveBtn')?.addEventListener('click', () => this.savePricing());

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
                const fabricUsage = CURTAIN_FABRIC_MATRIX.getFabricUsage(w, h);

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

                // Redirect to matrix view after 2 seconds
                setTimeout(() => {
                    window.location.href = '/PriceList/ManageRangesByDimensionsMatrix?itemId=' + this.config.itemId;
                }, 2000);
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
