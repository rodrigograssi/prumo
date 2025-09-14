window.chartFunctions = {
    createRadarChart: function(canvasId, data, options) {
        try {
            console.log('Criando gráfico radar para canvas:', canvasId);
            console.log('Dados:', data);
            console.log('Opções:', options);
            
            const canvas = document.getElementById(canvasId);
            if (!canvas) {
                console.error('Canvas não encontrado:', canvasId);
                return false;
            }
            
            const ctx = canvas.getContext('2d');
            
            // Destruir chart existente se houver
            if (canvas.chart) {
                canvas.chart.destroy();
                delete canvas.chart;
            }
            
            // Verificar se Chart.js está disponível
            if (typeof Chart === 'undefined') {
                console.error('Chart.js não está carregado');
                return false;
            }
            
            // Ajustar opções para remover callback problemático
            if (options.scales && options.scales.r && options.scales.r.ticks) {
                delete options.scales.r.ticks.callback;
            }
            
            // Criar novo chart
            canvas.chart = new Chart(ctx, {
                type: 'radar',
                data: data,
                options: options
            });
            
            console.log('Gráfico criado com sucesso');
            return true;
        } catch (error) {
            console.error('Erro ao criar gráfico:', error);
            return false;
        }
    },
    
    destroyChart: function(canvasId) {
        try {
            const canvas = document.getElementById(canvasId);
            if (canvas && canvas.chart) {
                canvas.chart.destroy();
                delete canvas.chart;
            }
        } catch (error) {
            console.error('Erro ao destruir gráfico:', error);
        }
    }
};

// Função global para capturar gráfico radar como imagem
window.captureRadarChart = function() {
    try {
        // Procurar o primeiro canvas que contém um gráfico radar ativo
        const canvases = document.querySelectorAll('canvas');
        for (let canvas of canvases) {
            if (canvas.chart && canvas.chart.config.type === 'radar') {
                return canvas.toDataURL('image/png');
            }
        }
        
        console.warn('Nenhum gráfico radar encontrado');
        return '';
    } catch (error) {
        console.error('Erro ao capturar gráfico:', error);
        return '';
    }
};

// Função global para fazer download de arquivo
window.downloadFile = function(fileData, fileName, mimeType) {
    try {
        // Converter byte array para blob
        const byteArray = new Uint8Array(fileData);
        const blob = new Blob([byteArray], { type: mimeType });
        
        // Criar URL temporária
        const url = window.URL.createObjectURL(blob);
        
        // Criar link de download
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        
        // Adicionar ao DOM temporariamente e clicar
        document.body.appendChild(link);
        link.click();
        
        // Limpar
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        
        console.log('Download iniciado:', fileName);
    } catch (error) {
        console.error('Erro ao fazer download:', error);
    }
};