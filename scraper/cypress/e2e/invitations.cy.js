const currentYear = new Date().getFullYear().toString();
const additional = ["Geburtsdatum", "Benutzer-Id"];
const paras = Cypress.env("flags");
const group = paras.group;

describe("automate invitations", function () {
  it("An-/Abmeldungen " + group + "; Jahr: " + paras.year, () => {
    cy.login();
    cy.getMainLink('Termine').click();
    cy.getByText('Jahr', '.NK1446B-k-d-com-clubdesk-gxt-theme-client-button-ClubDeskButtonCellAppearance-ClubDeskButtonStyle-buttonInner')
      .click();
    if (paras.year) {
      const desired = parseInt(paras.year);
      const current = parseInt(currentYear);
      const diff = currentYear - desired;
      cy.log("diff:", diff);
      if (diff > 0) {
        for (let i=0; i< diff; i++) {
          cy.getByText('<').click()
        }
      }
    }
    cy.getByText("Liste").click()
    cy.getByText('Alle anzeigen').click({force: true})
    cy.getByText('Alle anzeigen').click({force: true})
    cy.getByText(group).click()
    cy.getByText(group).then($el => {
      cy.log($el)
      cy.log($el.length)
      if ($el[0].tagName.toLowerCase() === 'label') {
        // if there are no event-entries, the checkbox with the groupname is found
        cy.log('ITEMS NOT FOUND')
        cy.writeFile("cypress/downloads/Export.csv", "")
        setTimeout(() => { throw new Error("OK, no Error: No Events found") }, 500)
      } else {
        cy.wrap($el).type("{ctrl}a")
      }
    })
    const menuItemClass = ".NK1446B-W-b-com-clubdesk-gxt-theme-client-base-menu-Css3MenuItemAppearance-Css3MenuItemStyle-menuItem"
    cy.getByText("An-/Abmeldungen").click()
    cy.viewport(1200, 1000)
    cy.getByText("Export / Drucken").click()
    cy.getByText("Zusätzliche Kontaktspalten").then($additional => cy.wrap($additional).realHover())
    for (let i=0; i<additional.length; i++) {
      // NK1446B-W-b-com-clubdesk-gxt-theme-client-base-menu-Css3MenuItemAppearance-Css3MenuItemStyle-menuItem
      cy.getByText(additional[i], menuItemClass).click()
    }
    cy.getByText("Export", menuItemClass).click({ force: true })
  });
});
