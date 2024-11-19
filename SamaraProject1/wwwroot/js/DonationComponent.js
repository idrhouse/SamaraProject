// DonationComponent.js

class DonationComponent {
    constructor() {
        this.amount = '';
        this.name = '';
        this.email = '';
        this.message = '';
    }

    render() {
        const component = document.createElement('div');
        component.className = 'min-h-screen bg-gradient-to-b from-blue-100 to-white py-12 px-4 sm:px-6 lg:px-8';
        component.innerHTML = `
      <div class="max-w-4xl mx-auto">
        
        <div class="text-center mb-12">
          <h1 class="text-4xl font-extrabold text-gray-900 sm:text-5xl md:text-6xl">
            <span class="block">Apoya a</span>
            <span class="block text-blue-600">Sámara Vive</span>
          </h1>
          <p class="mt-3 max-w-md mx-auto text-base text-gray-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
            Tu donación ayuda a fortalecer nuestra feria, hacerla mas grande y promover el desarrollo local.
          </p>
        </div>

        <div class="bg-white shadow-xl rounded-lg overflow-hidden mb-8">
          <div class="p-6 sm:p-10">
            <h2 class="text-3xl font-bold text-gray-900 mb-8 text-center">Opciones de Donación</h2>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <div class="bg-gradient-to-br from-blue-50 to-blue-100 shadow-lg rounded-lg p-6 transition-all duration-300 hover:shadow-xl">
                <h3 class="text-xl font-semibold mb-4 flex items-center text-blue-800">
                  <svg class="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"></path>
                  </svg>
                  Transferencia Bancaria
                </h3>
                <p class="text-gray-700 mb-4">
                  Puedes realizar tu donación mediante transferencia bancaria a las siguientes cuentas:
                </p>
                <ul class="list-disc pl-5 text-gray-600">
                  <li>Banco Nacional: XXXX-XXXX-XXXX-XXXX</li>
                  <li>Banco de Costa Rica: YYYY-YYYY-YYYY-YYYY</li>
                </ul>
              </div>

              <div class="bg-gradient-to-br from-blue-50 to-blue-100 shadow-lg rounded-lg p-6 transition-all duration-300 hover:shadow-xl">
                <h3 class="text-xl font-semibold mb-4 flex items-center text-blue-800">
                  <svg class="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                  </svg>
                  SINPE Móvil
                </h3>
                <p class="text-gray-700 mb-4">
                  Para donaciones rápidas, puedes usar SINPE Móvil:
                </p>
                <p class="text-2xl font-semibold text-blue-600">8824 2483</p>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white shadow-xl rounded-lg overflow-hidden mb-8">
          <div class="p-6 sm:p-10">
            <h2 class="text-3xl font-bold text-gray-900 mb-8 text-center">Síguenos en Redes Sociales</h2>
            <div class="flex justify-center space-x-6">
              <a href="https://www.facebook.com/feriasamara/" target="_blank" rel="noopener noreferrer" class="text-blue-600 hover:text-blue-800 transition-colors duration-300">
                <svg class="w-12 h-12" fill="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                  <path fill-rule="evenodd" d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z" clip-rule="evenodd" />
                </svg>
              </a>
              <a href="https://www.instagram.com/feriasamara/" target="_blank" rel="noopener noreferrer" class="text-pink-600 hover:text-pink-800 transition-colors duration-300">
                <svg class="w-12 h-12" fill="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                  <path fill-rule="evenodd" d="M12.315 2c2.43 0 2.784.013 3.808.06 1.064.049 1.791.218 2.427.465a4.902 4.902 0 011.772 1.153 4.902 4.902 0 011.153 1.772c.247.636.416 1.363.465 2.427.048 1.067.06 1.407.06 4.123v.08c0 2.643-.012 2.987-.06 4.043-.049 1.064-.218 1.791-.465 2.427a4.902 4.902 0 01-1.153 1.772 4.902 4.902 0 01-1.772 1.153c-.636.247-1.363.416-2.427.465-1.067.048-1.407.06-4.123.06h-.08c-2.643 0-2.987-.012-4.043-.06-1.064-.049-1.791-.218-2.427-.465a4.902 4.902 0 01-1.772-1.153 4.902 4.902 0 01-1.772-1.772c-.247-.636-.416-1.363-.465-2.427-.047-1.024-.06-1.379-.06-3.808v-.63c0-2.43.013-2.784.06-3.808.049-1.064.218-1.791.465-2.427a4.902 4.902 0 011.153-1.772A4.902 4.902 0 015.45 2.525c.636-.247 1.363-.416 2.427-.465C8.901 2.013 9.256 2 11.685 2h.63zm-.081 1.802h-.468c-2.456 0-2.784.011-3.807.058-.975.045-1.504.207-1.857.344-.467.182-.8.398-1.15.748-.35.35-.566.683-.748 1.15-.137.353-.3.882-.344 1.857-.047 1.023-.058 1.351-.058 3.807v.468c0 2.456.011 2.784.058 3.807.045.975.207 1.504.344 1.857.182.466.399.8.748 1.15.35.35.683.566 1.15.748.353.137.882.3 1.857.344 1.054.048 1.37.058 4.041.058h.08c2.597 0 2.917-.01 3.96-.058.976-.045 1.505-.207 1.858-.344.466-.182.8-.398 1.15-.748.35-.35.566-.683.748-1.15.137-.353.3-.882.344-1.857.048-1.055.058-1.37.058-4.041v-.08c0-2.597-.01-2.917-.058-3.96-.045-.976-.207-1.505-.344-1.858a3.097 3.097 0 00-.748-1.15 3.098 3.098 0 00-1.15-.748c-.353-.137-.882-.3-1.857-.344-1.023-.047-1.351-.058-3.807-.058zM12 6.865a5.135 5.135 0 110 10.27 5.135 5.135 0 010-10.27zm0 1.802a3.333 3.333 0 100 6.666 3.333 3.333 0 000-6.666zm5.338-3.205a1.2 1.2 0 110 2.4 1.2 1.2 0 010-2.4z" clip-rule="evenodd" />
                </svg>
              </a>
            </div>
          </div>
        </div>
      </div>
    `;

        return component;
    }
}

// Usage
window.addEventListener('load', () => {
    setTimeout(() => {
        const donationComponent = new DonationComponent();
        const container = document.getElementById('donation-container');
        if (container) {
            container.appendChild(donationComponent.render());
        }
    }, 100); // Small delay to ensure tab is ready
});