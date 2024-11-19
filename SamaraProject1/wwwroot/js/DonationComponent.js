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
        component.className = 'h-screen bg-gradient-to-b from-blue-100 to-white py-12 px-4 sm:px-6 lg:px-8';
        component.innerHTML = `
      <div class="max-w-4xl mx-auto">

  <div class="text-center mb-2"> <!-- Aumenté el margen aquí -->
    <h1 class="text-4xl font-extrabold text-gray-900 sm:text-5xl md:text-6xl">
      <span class="block">Apoya a</span>
      <span class="block text-blue-600">Sámara Vive</span>
    </h1>
    <p class="mt-3 max-w-md mx-auto text-base text-gray-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
      Tu donación ayuda a fortalecer nuestra feria, hacerla más grande y promover el desarrollo local.
    </p>
  </div>

  <!-- Contenedor para centrar el div de Transferencia Bancaria -->
  <div class="flex items-center justify-center h-screen py-2"> <!-- Añadí padding aquí para controlar el espaciado -->
    <div class="bg-white shadow-xl rounded-lg overflow-hidden mb-8 max-w-4xl w-full">
      <div class="p-6 sm:p-10">
        <h2 class="text-3xl font-bold text-gray-900 mb-8 text-center">Opciones de Donación</h2>

        <div class="flex items-center justify-center grid-cols-1 md:grid-cols-2 gap-8">
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
              <li>Asociación Samara Viva</li>
              <li>IBAN CR: 67015201001050412526</li>
            </ul>
          </div>
        </div>
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