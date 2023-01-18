const currentYear = new Date().getFullYear().toString();
const additional = ["Geburtsdatum", "Benutzer-Id"];
const paras = Cypress.env("flags");
const group = paras.group;

describe("automate invitations", function () {
  it("An-/Abmeldungen " + group + "; Jahr: " + paras.year, () => {
    cy.login();
    cy.get('div[qtip="Termine"]').click();
    cy.clickLink("Jahr");
    cy.wait(1000);
    if (paras.year) {
      const desired = parseInt(paras.year);
      const current = parseInt(currentYear);
      const diff = currentYear - desired;
      cy.log("diff:", diff);
      if (diff > 0) {
        cy.get(
          "table.GIIYKVVNB-com-clubdesk-client-application-bundle-css-EventsCss-calendarNavigation.cd-panel-header-center-tool"
        ).then(($tbls) => {
          const $tbl = $tbls[0];
          for (let i = 0; i < diff; i++) {
            cy.wrap(
              $tbl.childNodes[1].childNodes[0].childNodes[0].childNodes[0]
            ).click();
          }
        });
      }
    }
    cy.wait(1000);
    cy.clickLink("Liste")
    cy.wait(1000);
    // select event group
    cy.clickCheckboxLabel("Alle anzeigen")
    cy.clickCheckboxLabel("Alle anzeigen")
    cy.clickCheckboxLabel(group);
    cy.wait(200);
    // select all items
    cy.get('body').then($b => {
      var jqSel = Cypress.$(".GIIYKVVCIC-com-sencha-gxt-theme-blue-client-grid-BlueGridAppearance-BlueGridStyle-cell.GIIYKVVA3C-com-sencha-gxt-widget-core-client-grid-GridView-GridStateStyles-cell.x-grid-cell-first")
      cy.log(jqSel.length)
      if (jqSel.length < 1) {
        cy.log('ITEMS NOT FOUND')
        cy.writeFile("cypress/downloads/Export.csv", "")
        setTimeout(() => { throw new Error("OK, no Error: No Events found") }, 500)
      }
    })
    cy.wait(200)
    cy.get(
      "td.GIIYKVVCIC-com-sencha-gxt-theme-blue-client-grid-BlueGridAppearance-BlueGridStyle-cell.GIIYKVVA3C-com-sencha-gxt-widget-core-client-grid-GridView-GridStateStyles-cell.x-grid-cell-first"
    ).then(($firstCells) => {
      if ($firstCells.length === 1) {
        cy.wrap($firstCells).click()
      } else {
        cy.wrap($firstCells[0])
          .click()
          .then(($firstCell) => {
            cy.wrap($firstCell).type("{ctrl}a")
          });
      }
    })
    cy.clickLink("An-/Abmeldungen")
    cy.viewport(1200, 1000)
    cy.wait(2000)
    cy.clickCheckboxLabel("An-/Abmeldungen einblenden")
    cy.clickLink("Export / Drucken")
    cy.wait(500)
    cy.get(
      "span.GIIYKVVHKC-com-sencha-gxt-theme-blue-client-menu-BlueMenuItemAppearance-BlueMenuItemStyle-menuItem"
    ).then(($items) => {
      for (let i = 0; i < $items.length; i++) {
        const $item = $items[i];
        if ($item.innerText === "Zusätzliche Kontaktspalten") {
          cy.wrap($item).realHover();
          break;
        }
      }
    })

    cy.wait(200);
    cy.get(
      "span.GIIYKVVHKC-com-sencha-gxt-theme-blue-client-menu-BlueMenuItemAppearance-BlueMenuItemStyle-menuItem"
    ).then(($mis) => {
      var exportButton = null;
      for (let i = 0; i < $mis.length; i++) {
        const $mi = $mis[i];
        if ($mi.innerText === "Export") {
          exportButton = $mi;
          continue;
        }
        if (additional.filter((a) => a === $mi.innerText).length === 1) {
          if ($mi.childNodes.length > 1) {
            cy.wrap($mi.childNodes[1]).click();
          }
        }
      }
      if (exportButton != null) {
        cy.wrap(exportButton).click();
      }
    });
  });
});
