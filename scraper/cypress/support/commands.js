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

function log($elements) {
  cy.log('Number of elements found:', $elements.length);
  $elements.each((index, element) => {
    const tagName = element.tagName;
    const classNames = Array.from(element.classList).join(', ');
    const innerText = element.innerText
    cy.log(`Element ${index + 1}: TagName=${tagName}, ClassNames=${classNames}, InnerText=${innerText}`);
  });
}

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

Cypress.Commands.add('getMainLink', (txt) => {
  return cy.get(`div[qtip="${txt}"]`)
})

Cypress.Commands.add('getByText', (innerText, selector) => {
  if (selector) {
    cy.get(selector).then($elements => {
      const $filtered = $elements.filter(`:contains("${innerText}")`)
      log($filtered)
      return cy.wrap($filtered)
    })
  } else {
    cy.contains(innerText).then(($elements) => {
        log($elements)
        return cy.wrap($elements);
      });
  }
});
