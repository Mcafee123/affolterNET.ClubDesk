const currentYear = new Date().getFullYear().toString();
const additional = ["Geburtsdatum", "Benutzer-Id"];
const paras = Cypress.env("flags");

// const group = paras.group;
// const year = paras.year;

const group = "Training 1. Mannschaft"
const year = "2023"

describe("automate invitations", function () {
  it("An-/Abmeldungen " + group + "; Jahr: " + year, () => {
    cy.login();
    cy.viewport(1600, 1200)
    cy.getMainLink('Termine').click();
    cy.getByText('Jahr', '.NK1446B-k-d-com-clubdesk-gxt-theme-client-button-ClubDeskButtonCellAppearance-ClubDeskButtonStyle-buttonInner')
      .click();
    if (year) {
      const desired = parseInt(year);
      const current = parseInt(currentYear);
      const diff = currentYear - desired;
      cy.log("diff:", diff);
      if (diff > 0) {
        for (let i=0; i< diff; i++) {
          cy.getByText('<').click()
          cy.wait(200)
        }
      }
    }
    cy.wait(500)
    cy.getByText("Liste").click()
    cy.wait(200)
    cy.getByText('Alle anzeigen').click()
    cy.wait(200)
    cy.getByText('Alle anzeigen').click()
    cy.wait(200)
    cy.getByText(group).click()
    cy.wait(200)
    cy.getByText(group).then($el => {
      cy.log($el)
      cy.log($el.length)
      if ($el[0].tagName.toLowerCase() === 'label') {
        // if there are no event-entries, the checkbox on the left with the groupname is found
        cy.log('ITEMS NOT FOUND')
        cy.writeFile("cypress/downloads/Export.csv", "")
        setTimeout(() => { throw new Error("OK, no Error: No Events found") }, 500)
      } else {
        // otherwise, select all entries in the table
        cy.wrap($el).type("{ctrl}a")
      }
    })
    const menuItemClass = ".NK1446B-W-b-com-clubdesk-gxt-theme-client-base-menu-Css3MenuItemAppearance-Css3MenuItemStyle-menuItem"
    cy.getByText("An-/Abmeldungen").click()
    cy.wait(200)
    cy.getByText("Export / Drucken").click()
    cy.wait(200)
    cy.getByText("Zusätzliche Kontaktspalten").then($additional => cy.wrap($additional).realHover())
    for (let i=0; i<additional.length; i++) {
      cy.getByText(additional[i], menuItemClass).then($span => {
        cy.log('evaluate img-tag to see if it\'s checked already')
        if ($span[0].childNodes.length < 2 || $span[0].childNodes[1].tagName?.toLowerCase() !== 'img') {
          setTimeout(() => { throw new Error(`Image not found: ${additional[i]}`) }, 500)
        }
        const img = $span[0].childNodes[1]
        const style = img.getAttribute('style')
        console.log(style)
        const checked = style.startsWith('width:16px;height:16px;background:url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAwElEQVR4XmNgGDRgxowZzUB8nwR8Zvbs2TLIBlwA4jtAPJ8Qnjl')
        cy.log(style, 'checked:', checked)
        if (!checked){
          cy.log(`click ${additional[i]}`)
          cy.wrap($span[0]).click()
        }
      })
    }
    cy.getByText("Zusätzliche Kontaktspalten").then($additional => cy.wrap($additional).realHover())
    cy.getByText("Export", menuItemClass).click({ force: true })
  });
});
