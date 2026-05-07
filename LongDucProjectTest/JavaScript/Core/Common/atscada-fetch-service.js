
export class AtscadaFetchService {
    
    async handler(url, options = {}) {
        try {
            const { timeout = 3000 } = options;
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), timeout);
            
            const response = await fetch(url, {
                ...options,
                signal: controller.signal
            });

            clearTimeout(timeoutId);
            if (!response.ok) {
                const error = new Error(response.statusText);
                throw error;
            }
            return response.json();
        }
        catch {
            return null;
        }
    }
}