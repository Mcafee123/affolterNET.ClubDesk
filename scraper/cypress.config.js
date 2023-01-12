const { defineConfig } = require("cypress")
const path = require("path")
const fs = require("fs")

module.exports = defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
      on('task', {
        renameFile: (year) => {
          console.log(__dirname)
          const newName = `${__dirname}/cypress/downloads/Export-${year}.csv`
          console.log('rename')
          fs.rename(`${__dirname}/cypress/downloads/Export.csv`, newName, error => {
            if (error) {
              console.log(error)
            } else {
              console.log(`renamed to ${newName}`)
            }
          })
          return null
        },
      })
    },
  },
})
