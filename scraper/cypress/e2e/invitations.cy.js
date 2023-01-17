const currentYear = new Date().getFullYear().toString()
const additional = ['Geburtsdatum', 'Benutzer-Id']
const paras = Cypress.env('flags')

describe('automate invitations', function() {

  it('An-/Abmeldungen', () => {
    cy.login()
    cy.get('div[qtip="Termine"]').click()
    cy.clickLink('Jahr')
    cy.wait(1000)
    if (paras.year) {
      const desired = parseInt(paras.year)
      const current = parseInt(currentYear)
      const diff = currentYear - desired
      cy.log('diff:', diff)
      if (diff > 0) {        
        cy.get('table.GIIYKVVNB-com-clubdesk-client-application-bundle-css-EventsCss-calendarNavigation.cd-panel-header-center-tool').then($tbls => {
          const $tbl = $tbls[0]
          for (let i=0; i<diff;i++) {
            console.log($tbl.childNodes[1].childNodes[0].childNodes[0].childNodes[0])
            cy.wrap($tbl.childNodes[1].childNodes[0].childNodes[0].childNodes[0]).click()
          }
        })
      }
    }
    cy.wait(1000)
    cy.clickLink('Liste')
    cy.wait(2000)
    // select additional items
    cy.get('.GIIYKVVGIC-com-sencha-gxt-theme-blue-client-grid-BlueGridAppearance-BlueGridStyle-dataTable').then($tbls => {
      for (let i=0; i<$tbls.length; i++) {
        const $tbl = $tbls[i]
        if ($tbl.childNodes.length === 2 && $tbl.childNodes[1].childNodes.length > 1) {
          cy.wrap($tbl.childNodes[1].childNodes[0]).click().then($firstRow => {
            cy.wrap($firstRow).type("{ctrl}a")
          })
          break
        }
      }
    })
    cy.clickLink('An-/Abmeldungen')
    cy.viewport(1200, 1000)
    cy.wait(2000)
    cy.get('.GIIYKVVNHB-com-sencha-gxt-theme-base-client-field-CheckBoxDefaultAppearance-CheckBoxStyle-checkBoxLabel').then($cbs => {
      for (let i=0; i<$cbs.length; i++) {
        const $cb = $cbs[i]
        if ($cb.innerText === 'An-/Abmeldungen einblenden') {
          $cb.click()
          console.log('F O U N D')
          break
        }
      }
    })
    cy.clickLink('Export / Drucken')
    cy.wait(200)
    cy.get('span.GIIYKVVHKC-com-sencha-gxt-theme-blue-client-menu-BlueMenuItemAppearance-BlueMenuItemStyle-menuItem').then($items => {
      for (let i=0; i< $items.length; i++) {
        const $item = $items[i]
        if ($item.innerText === 'Zusätzliche Kontaktspalten') {
          cy.wrap($item).realHover()
          break
        }
      }
    })

    cy.wait(200)
    cy.get('span.GIIYKVVHKC-com-sencha-gxt-theme-blue-client-menu-BlueMenuItemAppearance-BlueMenuItemStyle-menuItem').then($mis => {
      var exportButton = null
      for (let i=0; i< $mis.length; i++) {
        const $mi = $mis[i]
        if ($mi.innerText === 'Export') {
          exportButton = $mi
          continue
        }
        if (additional.filter( a => a === $mi.innerText).length === 1) {
          console.log('F O U N D')
          if ($mi.childNodes.length > 1) {
            $mi.childNodes[1].click()
          }
        }
      }
      if (exportButton != null) {
        cy.wrap(exportButton).click()
        cy.wait(1000)      
      }
    })
  })
})