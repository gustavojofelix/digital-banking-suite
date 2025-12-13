describe('IAM Portal Auth Flows', () => {
  const baseUrl = 'http://localhost:4200';

  // Adjust these to match your seeded IAM admin user in backend
  const devUserEmail = 'iam.admin@alvorbank.test';
  const devUserPassword = 'Admin123456!';

  it('should display the login page', () => {
    cy.visit(`${baseUrl}/login`);

    cy.contains('Sign in to Alvor Bank').should('be.visible');
    cy.get('input[type="email"]').should('exist');
    cy.get('input[type="password"]').should('exist');
  });

  it('should show validation errors on empty submit', () => {
    cy.visit(`${baseUrl}/login`);

    cy.get('button[type="submit"]').click();

    cy.contains('Please enter a valid email address.').should('be.visible');
    cy.contains('Your password must be at least 8 characters long.').should(
      'be.visible'
    );
  });

  // it('should allow login for a seeded user (no 2FA)', () => {
  //   cy.visit(`${baseUrl}/login`);

  //   cy.get('input[type="email"]').type(devUserEmail);
  //   cy.get('input[type="password"]').type(devUserPassword);
  //   cy.get('button[type="submit"]').click();

  //   // Expect redirect to /me/security
  //   cy.url().should('include', '/me/security');
  //   cy.contains('My security').should('be.visible');
  // });

  it('should show generic success message on forgot password', () => {
    cy.visit(`${baseUrl}/forgot-password`);

    cy.get('input[type="email"]').type(devUserEmail);
    cy.get('button[type="submit"]').click();

    cy.contains('Check your email').should('be.visible');
    cy.contains(
      'If this email exists in our system, we sent you a link with instructions to reset your password.'
    ).should('be.visible');
  });
});
