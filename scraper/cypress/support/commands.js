// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })

Cypress.Commands.add('login', () => {
  const uri = 'https://app.clubdesk.com'
  const paras = Cypress.env('flags')
  cy.log(paras.email)
  cy.visit(uri)
  cy.get('#userId').type(paras.email)
  cy.get('#password').type(paras.pw)
  cy.get('.cdButtonPrimary').click()
  cy.wait(3000)
})

Cypress.Commands.add('clickLink', (txt) => {
  cy.get('div.GIIYKVVBFB-com-sencha-gxt-theme-base-client-button-ButtonCellDefaultAppearance-ButtonCellStyle-text').then($bts => {
    for (let i=0; i< $bts.length; i++) {
      if ($bts[i].innerText === txt) {
        console.log($bts[i].innerText)
        $bts[i].click()
        break
      }
    }
  })
})

Cypress.Commands.add('getYear', () => {
  cy.get('div.GIIYKVVFNC-com-sencha-gxt-theme-blue-client-panel-BlueHeaderAppearance-BlueHeaderStyle-headerText').then(function($divs) {
    for (let i = 0; i<$divs.length; i++) {
      const $div = $divs[i]
      if ($div.id.endsWith('-20-label')) {
        const y = $div.innerText
        cy.log(y.length)
        if (y.length > 6) {
          const year = y.substring(6)
          cy.log('Year:', year)
          return cy.wrap(year)
        }
      }
    }
  })
})