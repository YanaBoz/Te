FROM mcr.microsoft.com/mssql/server:2022-latest

# ��������� ������������ � mssql-tools
USER root
RUN apt-get update \
    && apt-get install -y curl apt-transport-https \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/10/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql17 \
    && ACCEPT_EULA=Y apt-get install -y mssql-tools \
    && echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

# ���������, ��� SQL Server ���������� ��� ������ ����������
ENTRYPOINT ["/opt/mssql/bin/sqlservr"]