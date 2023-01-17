// ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'

import 'cypress-real-events'

// Alternatively you can use CommonJS syntax:
// require('./commands')

Cypress.on('uncaught:exception', (err, runnable) => {
  // returning false here prevents Cypress from
  // failing the test
  
  // ignoring mouseflow-error
  if (err.name === 'ReferenceError' && err.toString().includes('> mouseflow is not defined')) {
    return false
  }

  // ignoring onpageload-error
  if (err.name === 'TypeError' && err.toString().includes('> Cannot read properties of undefined (reading \'onpageload\')')){
    return false
  }
})

