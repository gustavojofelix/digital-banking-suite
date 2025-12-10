import { getGreeting } from '../support/app.po';

describe('iam-portal', () => {
  beforeEach(() => {
    // Visiting root should redirect to the login page
    cy.visit('/');
  });

  it('should redirect to login page and show login title', () => {
    // Assert we are on /login
    cy.url().should('include', '/login');

    // Assert the login page title exists
    cy.contains('h1', 'Login');
  });
});
