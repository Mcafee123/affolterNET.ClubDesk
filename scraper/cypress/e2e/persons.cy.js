const currentYear = new Date().getFullYear().toString()
const additional = ['Geburtsdatum', 'Benutzer-Id']
const paras = Cypress.env('flags')

describe('automate persons', function() {

  it('Personen', () => {
    cy.login()
    cy.getMainLink('Kontakte und Gruppen').click()
    cy.getByText('Alle Kontakte').click()
    cy.wait(2000)
    cy.getByText('Export').click()
    cy.get('input[value="Sichtbare Spalten"]').click()
    cy.getByText('Alle Spalten').click()
    cy.getByText('OK').click()
  })
})
