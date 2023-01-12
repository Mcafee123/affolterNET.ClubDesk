const uri = 'https://app.clubdesk.com'
const currentYear = new Date().getFullYear().toString()
const paras = Cypress.env('flags')

const intercept = (method, url) => {
  let name = `${method}_${url.replace(/\//g, '_')}`
  name = name.replace(/\*/g, '')
  name = name.replace(/\?/g, '')
  name = name.replace(/\&/g, '')
  cy.intercept(method, url).as(name)
  return `@${name}`
}

describe('automate clubdesk: ' + paras.email, function() {

  it('An-/Abmeldungen', () => {
    cy.log(paras.email)
    cy.visit(uri)
    cy.get('#userId').type(paras.email)
    cy.get('#password').type(paras.pw)
    cy.get('.cdButtonPrimary').click()

    cy.wait(3000)
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
        if ($item.innerText === 'Export') {
          cy.wrap($item).click()
          break
        }
      }
    })
  })
})