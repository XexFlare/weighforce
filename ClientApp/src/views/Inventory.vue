<template>
  <div>
    <header>
      <img src="../assets/login.jpg" alt="login image" class="w-full" />
      <div class="w-full py-2 bg-primary text-gray-300 flex justify-end">
        <a href="/Identity/Account/Register?returnUrl=%2F"
        class="hover:bg-accent px-4 py-2 rounded"
          >Register</a
        >
        <a id="forgot-password" href="/Identity/Account/ForgotPassword"
        class="ml-4 hover:bg-accent px-4 py-2 rounded"
          >Forgot password?</a
        >
      </div>
    </header>

    <div>
      <main role="main" class="pb-3">
        <div>
          <div class="flex justify-center">
            <section class="w-64 mt-4 ">
              <form id="account" method="post">
                <hr />
                <div
                  class="text-danger validation-summary-valid"
                  data-valmsg-summary="true"
                >
                  <ul>
                    <li style="display: none"></li>
                  </ul>
                </div>
                <div>
                  <div class="border bg-gray-300">
                    📧
                    <input
                      class="bg-gray-300 w-10/12 focus:outline-none"
                      type="email"
                      data-val="true"
                      data-val-email="The Email field is not a valid e-mail address."
                      data-val-required="The Email field is required."
                      id="Input_Email"
                      name="Input.Email"
                      placeholder="Email Address"
                      value=""
                    />
                  </div>
                  <span
                    class="text-danger field-validation-valid"
                    data-valmsg-for="Input.Email"
                    data-valmsg-replace="true"
                  ></span>
                </div>
                <div class="mt-2 border bg-gray-300">
                  🔒
                  <input
                    class="bg-gray-300 w-10/12 focus:outline-none"
                    type="password"
                    data-val="true"
                    data-val-required="The Password field is required."
                    id="Input_Password"
                    name="Input.Password"
                    placeholder="Password"
                  />
                  <span
                    class="text-danger field-validation-valid"
                    data-valmsg-for="Input.Password"
                    data-valmsg-replace="true"
                  ></span>
                </div>
                <div class="mt-2">
                  <button
                    id="login-submit"
                    type="submit"
                    class="bg-primary text-white w-full py-2"
                  >
                    Log in
                  </button>
                </div>
                <div class="mt-2">
                  <div class="checkbox">
                    <label for="Input_RememberMe">
                      <input
                        class="text-primary"
                        type="checkbox"
                        data-val="true"
                        data-val-required="The Remember me? field is required."
                        id="Input_RememberMe"
                        name="Input.RememberMe"
                        value="true"
                      />
                      Remember me?
                    </label>
                  </div>
                </div>
                <input
                  name="__RequestVerificationToken"
                  type="hidden"
                  value="CfDJ8AnFEzafMP5NpEneqfS3_1TkdXO4btFZ_OXvwznBZgXz_N04mpw0JvBvh5y6BXyHSXW4woeAhMX1ZTUABCMBZYQ0bkufsVk2LqA_76HgaLkDC2epTp2lmgVC77lod8RRd5hXOoAXVDx5PQOJ85pO9fLFeepFYZXOriYq_b45yIX-5ZG2sw05i6KUh8LFDfblAg"
                />
                <input name="Input.RememberMe" type="hidden" value="false" />
              </form>
            </section>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>
