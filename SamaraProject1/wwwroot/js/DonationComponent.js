class DonationComponent {
    constructor() {
        this.sinpeNumber = "88630334" // Reemplaza esto con el número SINPE Móvil real
    }

    render() {
        const component = document.createElement("div")
        component.className =
            "min-h-screen bg-gradient-to-br from-orange-50 to-orange-100 py-6 sm:py-8 md:py-12 px-4 sm:px-6 lg:px-8"
        component.innerHTML = `
            <div class="max-w-3xl mx-auto">
                <div class="text-center mb-12">
                    <h1 class="text-3xl sm:text-4xl md:text-5xl font-extrabold text-gray-900">
                        <span class="block">Apoya a</span>
                        <span class="bg-clip-text text-transparent bg-gradient-to-r from-orange-500 to-orange-400">Sámara Vive</span>
                    </h1>
                    <p class="mt-4 max-w-2xl mx-auto text-xl text-gray-600">
                        Tu donación ayuda a fortalecer nuestra feria, hacerla más grande y promover el desarrollo local.
                    </p>
                </div>

                <div class="bg-white shadow-xl rounded-2xl overflow-hidden">
                    <div class="p-6 sm:p-10">
                        <h2 class="text-2xl sm:text-3xl font-bold text-gray-900 mb-8 text-center">Donar con SINPE Móvil</h2>

                        <div class="bg-gradient-to-br from-orange-50 to-orange-100 shadow-lg rounded-xl p-6 transition-all duration-300 hover:shadow-xl transform hover:scale-105">
                            <h3 class="text-xl font-semibold mb-4 flex items-center text-orange-700">
                                <svg class="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                                </svg>
                                SINPE Móvil
                            </h3>
                            <p class="text-gray-700 mb-4">
                                Puedes realizar tu donación fácilmente a través de SINPE Móvil:
                            </p>
                            <div class="bg-white rounded-lg p-4 text-center">
                                <p class="text-2xl font-bold text-orange-500">${this.sinpeNumber}</p>
                                <p class="text-sm text-gray-500 mt-2">Asociación Samara Viva</p>
                            </div>
                        </div>
                        
                        <div class="mt-10 text-center">
                            <p class="text-gray-500">Tu donación es una inversión en el crecimiento de nuestra feria y en el sueño de muchos emprendedores locales. Cada aporte se utiliza para mejorar las instalaciones, crear un espacio más cómodo y seguro para todos, y brindar más oportunidades a quienes luchan por sacar adelante sus negocios. Con tu ayuda, fortalecemos el comercio local, impulsamos la economía comunitaria y construimos juntos un lugar donde la creatividad, la cultura y el esfuerzo tienen un verdadero hogar.</p>
                        </div>
                    </div>
                </div>
            </div>
        `
        return component
    }
}

// Usage
window.addEventListener("load", () => {
    setTimeout(() => {
        const donationComponent = new DonationComponent()
        const container = document.getElementById("donation-container")
        if (container) {
            container.appendChild(donationComponent.render())
        }
    }, 100) // Small delay to ensure container is ready
})