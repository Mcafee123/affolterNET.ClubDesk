FROM swacli/static-web-apps-cli:1.1.4

USER root

WORKDIR /src

# copy stuff
RUN mkdir /src/scraper
COPY scraper/ /src/scraper
COPY _build.sh /src
RUN mkdir /src/console
COPY console /src/console
COPY __config.sh /src/
COPY run.sh /src/

# install
RUN cd scraper && npm install && npx cypress install

ENTRYPOINT ["/src/run.sh"]
