const currentYear = new Date().getFullYear().toString()
const additional = ['Geburtsdatum', 'Benutzer-Id']
const paras = Cypress.env('flags')

describe('automate persons', function() {

  it('Personen', () => {
    cy.login()
    cy.get('div[qtip="Kontakte und Gruppen"]').click()
    cy.get('span.GIIYKVVBDC-com-sencha-gxt-theme-base-client-tree-TreeBaseAppearance-TreeBaseStyle-text').then($divs => {
      for (var i=0; i<$divs.length; i++) {
        const $div = $divs[i]
        if ($div.innerText === 'Alle Kontakte') {
          cy.wrap($div).click()
        }
      }
    })
    cy.wait(2000)
    cy.clickLink('Export')
    cy.get('div.GIIYKVVPPB-com-sencha-gxt-theme-base-client-field-TriggerFieldDefaultAppearance-TriggerFieldStyle-wrap').then($divs => {
      for (var i=0; i<$divs.length; i++) {
        var $div = $divs[i]
        if ($div.childNodes.length > 0) {
          var $tbl = $div.childNodes[0] // table
          if ($tbl.childNodes.length > 0) {
            var $tbody = $tbl.childNodes[0] // tbody
            if ($tbody.childNodes.length > 0) {
              var $tr = $tbody.childNodes[0] // tr
              if ($tr.childNodes.length > 1) {
                var $td = $tr.childNodes[0] // td
                if ($td.childNodes.length > 0 && $td.childNodes[0].value === 'Sichtbare Spalten') {
                  var $sichtSpalten = $td.childNodes[0] // input
                  cy.wrap($sichtSpalten).realClick()
                  cy.wait(200)
                  break
                }
              }
            }
          }
        } 
      }
    })
    cy.wait(200)
    cy.get('div.GIIYKVVOYB-com-sencha-gxt-theme-base-client-listview-ListViewDefaultAppearance-ListViewDefaultStyle-item').then($divs => {
      for (var i=0; i<$divs.length; i++) {
        var $div = $divs[i]
        console.log($div)
        if ($div.innerText === 'Alle Spalten') {
          cy.wrap($div).realClick()
          cy.wait(200)
          break
        }
      }
    })
    cy.clickLink('OK')
  })
})
