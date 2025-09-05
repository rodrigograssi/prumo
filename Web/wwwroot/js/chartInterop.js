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